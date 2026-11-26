using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Events.Services.Translation;

// English title and description of an event (GS-REQ-010, 10.4, "Automatic translation"):
// - a text without Cyrillic is copied without a request;
// - a text with Cyrillic (also mixed BG/EN) comes from the cache by its hash, or is translated by the AI model;
// - a translation that still contains Cyrillic (or is empty or much longer than the source) is retried once and
//   then dropped; it is never transliterated.
// Registered as scoped: the time budget, the circuit breaker and the counters belong to one run (one scope).
// Safe for parallel calls within the run.
public sealed class EventTranslationService : IEventTranslationService
{
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 4000;
    private const int MaxConsecutiveApiFailures = 3;
    private const int MaxModelNameLength = 50;

    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);

    private readonly ClaudeTranslationClient _client;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TranslationOptions _options;
    private readonly ILogger<EventTranslationService> _logger;

    // Several events are translated at the same time through this one instance (UpcomingEventTranslator), so the
    // run state below and the Stats counters are shared between threads. A plain "++" or a check followed by a write
    // is not atomic: two threads could lose a count or both start the time budget. Every read and write of this
    // state therefore happens inside lock (_sync), and only there; nothing slow (requests, database) runs under the
    // lock, so it is held for microseconds while a request takes seconds.
    private readonly object _sync = new();

    // Run state, guarded by _sync.
    private DateTime? _firstRequestAt;
    private int _consecutiveApiFailures;
    private bool _stopReported;

    public EventTranslationService(
        ClaudeTranslationClient client,
        IServiceScopeFactory scopeFactory,
        IOptions<TranslationOptions> options,
        ILogger<EventTranslationService> logger)
    {
        _client = client;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    public TranslationRunStats Stats { get; } = new();

    public async Task<EventTranslation> TranslateAsync(string? name, string? description, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return EventTranslation.None;

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var sources = new Dictionary<string, string> { [ClaudeTranslationClient.TitleField] = name.Trim() };
            if (!string.IsNullOrWhiteSpace(description))
                sources[ClaudeTranslationClient.DescriptionField] = description.Trim();

            var results = await ResolveAsync(sources, cancellationToken);

            var nameEn = results.GetValueOrDefault(ClaudeTranslationClient.TitleField);
            if (nameEn is null)
                return EventTranslation.None;

            var descriptionEn = results.GetValueOrDefault(ClaudeTranslationClient.DescriptionField);

            return new EventTranslation(TruncateAtWord(nameEn, MaxNameLength),
                descriptionEn is null ? null : TruncateAtWord(descriptionEn, MaxDescriptionLength));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Translation failed for '{EventName}'; saved without an English title", name);
            return EventTranslation.None;
        }
        finally
        {
            Count(stats => stats.Elapsed += stopwatch.Elapsed);
        }
    }

    // Returns the English text for each field that has one; a field without a usable translation is missing.
    private async Task<Dictionary<string, string>> ResolveAsync(Dictionary<string, string> sources, CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, string>();
        var pending = new Dictionary<string, string>();

        foreach (var (field, text) in sources)
        {
            if (CyrillicTransliterator.ContainsCyrillic(text))
            {
                pending[field] = text;
            }
            else
            {
                results[field] = text;
                Count(stats => stats.Copied++);
            }
        }

        if (pending.Count == 0)
            return results;

        // Own scope: the caller may use its DbContext at the same time (categorization runs in parallel).
        using var scope = _scopeFactory.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<ITranslationCacheRepository>();

        var hashes = pending.ToDictionary(p => p.Key, p => ComputeHash(p.Key, p.Value));

        foreach (var (field, hash) in hashes)
        {
            var cached = await cache.GetAsync(hash);
            if (cached is null)
                continue;

            results[field] = cached.TranslatedText;
            pending.Remove(field);
            Count(stats => stats.FromCache++);
        }

        if (pending.Count == 0)
            return results;

        if (!CanSendRequest())
        {
            Count(stats => stats.Skipped += pending.Count);
            return results;
        }

        var translated = await TranslateWithValidationAsync(pending, sources[ClaudeTranslationClient.TitleField], cancellationToken);

        foreach (var (field, text) in translated)
        {
            results[field] = text;
            Count(stats => stats.Translated++);

            await cache.AddAsync(new TranslationCacheEntry
            {
                Hash = hashes[field],
                TranslatedText = text,
                Model = _options.Model.Length <= MaxModelNameLength ? _options.Model : _options.Model[..MaxModelNameLength]
            });
        }

        Count(stats => stats.Failed += pending.Count - translated.Count);
        return results;
    }

    // One request for all pending fields; the rejected ones are sent once more with the reason.
    private async Task<Dictionary<string, string>> TranslateWithValidationAsync(Dictionary<string, string> pending, string eventName, CancellationToken cancellationToken)
    {
        var accepted = new Dictionary<string, string>();
        var toSend = pending;
        string? correction = null;

        for (var attempt = 1; attempt <= 2 && toSend.Count > 0 && CanSendRequest(); attempt++)
        {
            var response = await _client.TranslateAsync(toSend, correction, cancellationToken);
            Count(stats =>
            {
                stats.InputTokens += response.InputTokens;
                stats.OutputTokens += response.OutputTokens;
            });

            if (response.IsApiError)
            {
                RecordRequestResult(succeeded: false);
                _logger.LogWarning("Translation request for '{EventName}' failed: {Error}", eventName, response.Error);
                break;
            }

            RecordRequestResult(succeeded: true);

            var rejected = new Dictionary<string, string>();
            var reasons = new List<string>();

            foreach (var (field, source) in toSend)
            {
                var text = response.Fields.GetValueOrDefault(field)?.Trim();
                var reason = response.Error ?? Validate(field, source, text);

                if (reason is null)
                {
                    accepted[field] = text!;
                }
                else
                {
                    rejected[field] = source;
                    reasons.Add(reason);
                }
            }

            if (rejected.Count > 0)
            {
                _logger.LogWarning("Translation of '{EventName}' rejected (attempt {Attempt}): {Reasons}", eventName, attempt, string.Join("; ", reasons));
            }

            toSend = rejected;
            correction = string.Join("; ", reasons.Distinct());
        }

        return accepted;
    }

    private static string? Validate(string field, string source, string? translation)
    {
        if (string.IsNullOrWhiteSpace(translation))
            return $"the {field} was empty";

        if (CyrillicTransliterator.ContainsCyrillic(translation))
            return $"the {field} still contained Cyrillic letters; the answer must not contain any";

        // A much longer answer is an explanation or an addition, not a translation.
        if (translation.Length > source.Length * 2 + 50)
            return $"the {field} was much longer than the source; return the translation only";

        if (field == ClaudeTranslationClient.TitleField && translation.Contains('\n'))
            return "the title must be a single line";

        return null;
    }

    // Requests stop for the rest of the run when translation is disabled, the time budget is used up or the API failed several times in a row.
    // The checks and the start of the time budget happen under one lock, so they are one atomic step.
    private bool CanSendRequest()
    {
        lock (_sync)
        {
            if (!_options.Enabled)
                return false;

            string? stopReason = null;

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                stopReason = "the Claude API key is not configured";
            else if (_consecutiveApiFailures >= MaxConsecutiveApiFailures)
                stopReason = $"{_consecutiveApiFailures} requests failed in a row";
            else if (_firstRequestAt is { } startedAt && DateTime.UtcNow - startedAt > TimeSpan.FromMinutes(_options.MaxRunMinutes))
                stopReason = $"the time budget of {_options.MaxRunMinutes} min is used up";

            if (stopReason is not null)
            {
                if (!_stopReported)
                {
                    _logger.LogWarning("Translation stopped for this run: {Reason}. The remaining events wait for the next run", stopReason);
                    _stopReported = true;
                }

                return false;
            }

            _firstRequestAt ??= DateTime.UtcNow;
            return true;
        }
    }

    // A failed request counts for the circuit breaker; a successful one resets it. Guarded by _sync.
    private void RecordRequestResult(bool succeeded)
    {
        lock (_sync)
        {
            _consecutiveApiFailures = succeeded ? 0 : _consecutiveApiFailures + 1;
        }
    }

    // Updates the shared counters. Guarded by _sync.
    private void Count(Action<TranslationRunStats> update)
    {
        lock (_sync)
        {
            update(Stats);
        }
    }

    // Lowercase hex SHA-256 of the field, the model and the text with collapsed whitespace.
    private string ComputeHash(string field, string text)
    {
        var normalized = Whitespace.Replace(text.Normalize(NormalizationForm.FormC), " ").Trim();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{field}|{_options.Model}|{normalized}"));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string TruncateAtWord(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;

        var cut = text.LastIndexOf(' ', maxLength - 1);
        return (cut > maxLength / 2 ? text[..cut] : text[..(maxLength - 1)]).TrimEnd() + "…";
    }
}

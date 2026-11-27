using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Events.Services.Translation;

// Translates the upcoming events without an English title (GS-REQ-010, 10.4), as the last step of a crawler run.
public interface IUpcomingEventTranslator
{
    // Nearest events first, until all are done, the time budget is used up or deadlineUtc (minus a safety margin)
    // comes. Never throws. Each event is saved right after its translation, so a stopped run loses at most the
    // events being translated at that moment; the next run continues with the events still without an English title.
    Task TranslateUpcomingAsync(DateTime deadlineUtc, CancellationToken cancellationToken = default);
}

public sealed class UpcomingEventTranslator : IUpcomingEventTranslator
{
    // More than one run can translate; the rest waits for the next run.
    private const int MaxEventsPerRun = 2000;

    private static readonly TimeSpan SafetyMargin = TimeSpan.FromMinutes(2);

    private readonly IEventTranslationService _translationService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISofiaClock _clock;
    private readonly TranslationOptions _options;
    private readonly ILogger<UpcomingEventTranslator> _logger;

    public UpcomingEventTranslator(
        IEventTranslationService translationService,
        IServiceScopeFactory scopeFactory,
        ISofiaClock clock,
        IOptions<TranslationOptions> options,
        ILogger<UpcomingEventTranslator> logger)
    {
        _translationService = translationService;
        _scopeFactory = scopeFactory;
        _clock = clock;
        _options = options.Value;
        _logger = logger;
    }

    public async Task TranslateUpcomingAsync(DateTime deadlineUtc, CancellationToken cancellationToken = default)
    {
        var budget = TimeSpan.FromMinutes(_options.MaxRunMinutes);
        var timeLeft = deadlineUtc - SafetyMargin - DateTime.UtcNow;
        if (timeLeft < budget)
            budget = timeLeft;

        if (budget <= TimeSpan.Zero)
        {
            _logger.LogWarning("No time left to translate upcoming events; they wait for the next run");
            return;
        }

        var startedAt = DateTime.UtcNow;
        var candidateCount = 0;
        var saved = 0;
        var withoutTranslation = 0;

        // The events are translated in parallel and every worker updates the two counters above. As in
        // EventTranslationService, the shared counters are changed only inside lock (countersSync).
        var countersSync = new object();

        using var budgetCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        budgetCts.CancelAfter(budget);

        try
        {
            var candidates = await GetCandidatesAsync(budgetCts.Token);
            candidateCount = candidates.Count;

            _logger.LogInformation("Translating {Count} upcoming events without an English title (budget {Budget}, {Parallel} in parallel)",
                candidateCount, budget, _options.MaxParallelRequests);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, _options.MaxParallelRequests),
                CancellationToken = budgetCts.Token
            };

            // Events with the same title and description (performances of one show on different dates) form one group.
            // The groups run in parallel and the events of a group one after another, so the first one translates the
            // texts and the others take them from the cache: no duplicate request and no duplicate cache entry.
            // GroupBy keeps the order of the first event of each group (nearest first).
            var groups = candidates.GroupBy(e => (e.Name, e.Description)).ToList();

            // Up to MaxParallelRequests groups at a time, taken in the order of the list. When the budget token is
            // cancelled, no new group or event is started, the running ones end (a finished translation is still
            // saved) and ForEachAsync then throws OperationCanceledException, handled below.
            await Parallel.ForEachAsync(groups, parallelOptions, async (group, token) =>
            {
                foreach (var eventEntity in group)
                {
                    token.ThrowIfCancellationRequested();

                    // The slow part (request and save) runs outside the lock, in parallel with the other groups.
                    var isSaved = await TranslateAndSaveAsync(eventEntity, token);

                    // Only the counter update is guarded: a short, atomic step shared by all workers.
                    lock (countersSync)
                    {
                        if (isSaved)
                            saved++;
                        else
                            withoutTranslation++;
                    }
                }
            });
        }
        catch (OperationCanceledException) when (budgetCts.IsCancellationRequested)
        {
            _logger.LogInformation("Translation of upcoming events stopped by the time budget; the rest continues in the next run");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Translation of upcoming events failed; the rest continues in the next run");
        }

        var stats = _translationService.Stats;
        _logger.LogInformation("Upcoming events translated: Saved={Saved}, WithoutTranslation={WithoutTranslation}, NotReached={NotReached}, " +
            "Time={Elapsed}; texts: Translated={Translated}, FromCache={FromCache}, Copied={Copied}, Failed={Failed}, " +
            "Skipped={Skipped}, InputTokens={InputTokens}, OutputTokens={OutputTokens}",
            saved, withoutTranslation, candidateCount - saved - withoutTranslation, DateTime.UtcNow - startedAt,
            stats.Translated, stats.FromCache, stats.Copied, stats.Failed, stats.Skipped, stats.InputTokens, stats.OutputTokens);
    }

    private async Task<List<Event>> GetCandidatesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

        return await repository.GetUpcomingWithoutEnglishTitleAsync(_clock.Today, MaxEventsPerRun, cancellationToken);
    }

    // True when the English texts were saved.
    private async Task<bool> TranslateAndSaveAsync(Event eventEntity, CancellationToken cancellationToken)
    {
        var translation = await _translationService.TranslateAsync(eventEntity.Name, eventEntity.Description, cancellationToken);
        if (translation.NameEn is null)
            return false;

        // Own scope per event: the DbContext is not shared between parallel calls. A finished translation is saved
        // even when the budget runs out at that moment.
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

            return await repository.SetEnglishTextsAsync(eventEntity.Id, translation.NameEn, translation.DescriptionEn, CancellationToken.None);
        }
        catch (Exception ex)
        {
            // One event that cannot be saved (e.g. a database error after the retries) must not stop the other events.
            // It stays without an English title and is taken again in the next run, where its texts come from the cache.
            _logger.LogWarning(ex, "Saving the English texts of event {EventId} failed; it is translated again in the next run", eventEntity.Id);
            return false;
        }
    }
}

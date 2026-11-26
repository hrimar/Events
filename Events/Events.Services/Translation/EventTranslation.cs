namespace Events.Services.Translation;

// English title and description of an event; null when there is no usable translation (GS-REQ-010, 10.4).
public sealed record EventTranslation(string? NameEn, string? DescriptionEn)
{
    public static EventTranslation None { get; } = new(null, null);
}

// Counters of one run, logged by the caller. Each title or description counts once.
public sealed class TranslationRunStats
{
    // Texts without Cyrillic, used as they are.
    public int Copied { get; set; }

    public int FromCache { get; set; }

    public int Translated { get; set; }

    // Rejected after the retry, or the request failed.
    public int Failed { get; set; }

    // Not sent: translation disabled, the time budget is used up or the circuit breaker is open.
    public int Skipped { get; set; }

    public long InputTokens { get; set; }

    public long OutputTokens { get; set; }

    // Summed over all calls; with parallel calls it is longer than the wall-clock time.
    public TimeSpan Elapsed { get; set; }
}

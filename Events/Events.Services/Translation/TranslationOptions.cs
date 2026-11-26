namespace Events.Services.Translation;

// Settings of the English translation of event titles and descriptions (GS-REQ-010, 10.4, "Automatic translation").
// Bound from the "Translation" section; the API key is the shared "Claude:ApiKey".
public sealed class TranslationOptions
{
    public const string SectionName = "Translation";

    // Turns the requests to the AI model on. Texts without Cyrillic are copied to the English fields either way.
    public bool Enabled { get; set; }

    public string Model { get; set; } = "claude-haiku-4-5-20251001";

    // Time budget of one run (one service scope), counted from its first request; the events left over are translated
    // in the next run.
    public int MaxRunMinutes { get; set; } = 8;

    public int RequestTimeoutSeconds { get; set; } = 20;

    // Requests sent at the same time when translating the upcoming events.
    public int MaxParallelRequests { get; set; } = 3;

    public string? ApiKey { get; set; }
}

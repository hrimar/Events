namespace Events.Services.Translation;

// Translates the title and description of an event into English (GS-REQ-010, 10.4). Never throws: on any problem
// the affected field is null. The English fields never contain Cyrillic.
public interface IEventTranslationService
{
    TranslationRunStats Stats { get; }

    // A title without a translation gives no English description either (the English page then stays Bulgarian).
    // A description whose translation was rejected stays null while the title is kept. A description that got no
    // answer (time budget, circuit breaker, failed request) gives no English texts at all, so the event is tried again.
    Task<EventTranslation> TranslateAsync(string? name, string? description, CancellationToken cancellationToken = default);
}

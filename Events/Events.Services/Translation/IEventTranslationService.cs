namespace Events.Services.Translation;

// Translates the title and description of an event into English (GS-REQ-010, 10.4). Never throws: on any problem
// the affected field is null. The English fields never contain Cyrillic.
public interface IEventTranslationService
{
    TranslationRunStats Stats { get; }

    // A title without a translation gives no English description either (the English page then stays Bulgarian).
    // A description without a translation stays null while the title is kept.
    Task<EventTranslation> TranslateAsync(string? name, string? description, CancellationToken cancellationToken = default);
}

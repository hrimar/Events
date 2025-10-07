using Events.Crawler.Models;
using Events.Models.Enums;

namespace Events.Crawler.Services.Interfaces;

public interface IAiTaggingService
{
    Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null);
    Task<EventCategory> ClassifyEventAsync(string eventName, string description);
    Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description);
}
using Events.Crawler.Models;
using Events.Models.Enums;

namespace Events.Crawler.Services.Interfaces;

public interface IAiTaggingService
{
    // Comprehensive method for unified processing
    Task<TaggingResult> ProcessEventComprehensivelyAsync(string eventName, string description, string? location = null);
}
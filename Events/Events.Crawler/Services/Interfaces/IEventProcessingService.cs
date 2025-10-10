using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Models.Entities;

namespace Events.Crawler.Services.Interfaces;

public interface IEventProcessingService
{
    Task<ProcessingResult> ProcessCrawledEventsAsync(IEnumerable<CrawledEventDto> crawledEvents);
    Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents);
    Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent);
    Task<Event> MapToEntityAsync(CrawledEventDto crawledEvent);
}
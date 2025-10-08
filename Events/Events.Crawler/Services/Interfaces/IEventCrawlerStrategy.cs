using Events.Crawler.DTOs.Common;
using Events.Crawler.Enums;

namespace Events.Crawler.Services.Interfaces;

public interface IEventCrawlerStrategy
{
    string SourceName { get; }
    CrawlerType CrawlerType { get; }
    Task<CrawlResult> CrawlAsync(DateTime? targetDate = null);
    bool IsHealthy();
}

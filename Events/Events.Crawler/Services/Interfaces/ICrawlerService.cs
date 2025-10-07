using Events.Crawler.DTOs.Common;

namespace Events.Crawler.Services.Interfaces;

public interface ICrawlerService
{
    Task<CrawlResult> CrawlEventsAsync(string source, DateTime? targetDate = null);
    Task<CrawlResult> CrawlAllSourcesAsync(DateTime? targetDate = null);
    IEnumerable<string> GetSupportedSources();
}
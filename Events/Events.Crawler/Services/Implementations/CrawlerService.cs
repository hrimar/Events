using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Events.Crawler.Enums;
using Events.Crawler.DTOs.Common;

namespace Events.Crawler.Services.Implementations;

public class CrawlerService : ICrawlerService
{
    private readonly IEnumerable<IEventCrawlerStrategy> _crawlers;
    private readonly ILogger<CrawlerService> _logger;

    public CrawlerService(IEnumerable<IEventCrawlerStrategy> crawlers, ILogger<CrawlerService> logger)
    {
        _crawlers = crawlers;
        _logger = logger;
    }

    public async Task<CrawlResult> CrawlEventsAsync(string source, DateTime? targetDate = null)
    {
        var crawler = _crawlers.FirstOrDefault(c => c.SourceName.Equals(source, StringComparison.OrdinalIgnoreCase));

        if (crawler == null)
        {
            return new CrawlResult
            {
                Source = source,
                Success = false,
                ErrorMessage = $"No crawler found for source: {source}"
            };
        }

        if (!crawler.IsHealthy())
        {
            return new CrawlResult
            {
                Source = source,
                Success = false,
                ErrorMessage = $"Crawler for {source} is not healthy"
            };
        }

        try
        {
            _logger.LogInformation("Starting crawl for source: {Source}", source);
            var result = await crawler.CrawlAsync(targetDate);
            _logger.LogInformation("Completed crawl for {Source}. Found {EventCount} events", source, result.EventsFound);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling source: {Source}", source);
            return new CrawlResult
            {
                Source = source,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<CrawlResult> CrawlAllSourcesAsync(DateTime? targetDate = null) 
    {
        var allResults = new List<CrawlResult>();
        var startTime = DateTime.UtcNow;

        foreach (var crawler in _crawlers.Where(c => c.IsHealthy()))
        {
            try
            {
                var result = await crawler.CrawlAsync(targetDate);
                allResults.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crawling source: {Source}", crawler.SourceName);
                allResults.Add(new CrawlResult
                {
                    Source = crawler.SourceName,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return new CrawlResult
        {
            Source = "All Sources",
            CrawledAt = DateTime.UtcNow,
            Success = allResults.Any(r => r.Success),
            EventsFound = allResults.Sum(r => r.EventsFound),
            EventsProcessed = allResults.Sum(r => r.EventsProcessed),
            EventsSkipped = allResults.Sum(r => r.EventsSkipped),
            Duration = DateTime.UtcNow - startTime,
            Events = allResults.SelectMany(r => r.Events).ToList()
        };
    }

    public IEnumerable<string> GetSupportedSources()
    {
        return _crawlers.Select(c => c.SourceName);
    }
}
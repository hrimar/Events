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
        var healthyCrawlers = _crawlers.Where(c => c.IsHealthy() ).ToList(); // && c.CrawlerType == CrawlerType.HttpApi

        // Throttle WebScraping crawlers (each launches its own Chromium process) to avoid
        // CPU/memory contention when too many browsers run at once in the same container.
        // HttpApi crawlers don't use a browser, so they run unthrottled.
        using var webScrapingThrottle = new SemaphoreSlim(2);

        // Running all crawlers in parallel instead of sequentially
        var crawlTasks = healthyCrawlers.Select(async crawler =>
        {
            var isWebScraping = crawler.CrawlerType == CrawlerType.WebScraping;
            if (isWebScraping)
                await webScrapingThrottle.WaitAsync();

            try
            {
                _logger.LogInformation("[{Source}] Starting parallel crawl", crawler.SourceName);
                var crawlResult = await crawler.CrawlAsync(targetDate);
                _logger.LogInformation("[{Source}] Crawl finished: Found={Found}", crawler.SourceName, crawlResult.EventsFound);
                return crawlResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Source}] Error crawling", crawler.SourceName);
                return new CrawlResult
                {
                    Source = crawler.SourceName,
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
            finally
            {
                if (isWebScraping)
                    webScrapingThrottle.Release();
            }
        });

        var results = await Task.WhenAll(crawlTasks);
        allResults.AddRange(results);

        var totalDuration = DateTime.UtcNow - startTime;
        _logger.LogInformation("All crawlers completed in {TotalDuration}. Total events found: {TotalEvents}", totalDuration, allResults.Sum(r => r.EventsFound));

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
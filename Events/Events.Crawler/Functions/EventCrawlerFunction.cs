using Events.Crawler.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Events.Crawler.Functions;

public class EventCrawlerFunction
{
    private readonly ICrawlerService _crawlerService;
    private readonly IEventProcessingService _eventProcessingService;
    private readonly ILogger<EventCrawlerFunction> _logger;

    public EventCrawlerFunction(
        ICrawlerService crawlerService,
        IEventProcessingService eventProcessingService,
        ILogger<EventCrawlerFunction> logger)
    {
        _crawlerService = crawlerService;
        _eventProcessingService = eventProcessingService;
        _logger = logger;
    }

    [Function("CrawlEventsFunction")]
    public async Task Run([TimerTrigger("0 0 4 * * *")] TimerInfo myTimer) // 4:00 AM daily
    {
        _logger.LogInformation("Event crawler function started at: {Time}", DateTime.UtcNow);

        try
        {
            // Crawl all sources
            var crawlResult = await _crawlerService.CrawlAllSourcesAsync();

            _logger.LogInformation("Crawling completed. Found {EventCount} events from {SourceCount} sources",
                crawlResult.EventsFound, _crawlerService.GetSupportedSources().Count());

            if (crawlResult.Success && crawlResult.Events.Any())
            {
                // Process and save events to database
                var processingResult = await _eventProcessingService.ProcessAndTagEventsAsync(crawlResult.Events);

                _logger.LogInformation("Processing completed. Created: {Created}, Updated: {Updated}, Skipped: {Skipped}",
                    processingResult.EventsCreated, processingResult.EventsUpdated, processingResult.EventsSkipped);

                if (processingResult.Errors.Any())
                {
                    _logger.LogWarning("Processing completed with errors: {Errors}",
                        string.Join(", ", processingResult.Errors));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during event crawling process");
            throw; // Re-throw to trigger Azure Function retry policy
        }

        _logger.LogInformation("Event crawler function completed at: {Time}", DateTime.UtcNow);
    }

    [Function("CrawlSpecificSourceFunction")]
    public async Task<HttpResponseData> CrawlSpecificSource(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "crawl/{source}")] HttpRequestData req, string source)
    {
        _logger.LogInformation("Manual crawl requested for source: {Source}", source);

        try
        {
            var result = await _crawlerService.CrawlEventsAsync(source);

            if (result.Success && result.Events.Any())
            {
                var processingResult = await _eventProcessingService.ProcessAndTagEventsAsync(result.Events);
                _logger.LogInformation("Manual crawl completed for {Source}. Processed {Count} events",
                    source, processingResult.EventsProcessed);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    success = true,
                    eventsProcessed = processingResult.EventsProcessed,
                    eventsCreated = processingResult.EventsCreated,
                    eventsUpdated = processingResult.EventsUpdated
                });
                return response;
            }
            else
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new
                {
                    success = false,
                    message = result.ErrorMessage ?? "No events found"
                });
                return response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual crawl for source: {Source}", source);

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new
            {
                success = false,
                error = ex.Message
            });
            return errorResponse;
        }
    }
}
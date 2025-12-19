using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task CrawlEventsTimerFunction([TimerTrigger("0 0 4 * * *")] TimerInfo myTimer)
    //public async Task CrawlEventsTimerFunction([TimerTrigger("0 27 19 * * *")] TimerInfo myTimer)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(8));

        _logger.LogInformation("Event crawler function started at: {Time}", DateTime.UtcNow);

        try
        {
            var crawlResult = await _crawlerService.CrawlAllSourcesAsync();
            _logger.LogInformation("Parallel crawling completed in {Duration}. Found {EventCount} events", crawlResult.Duration, crawlResult.EventsFound);

            if (crawlResult.Success && crawlResult.Events.Any())
            {
                var events = crawlResult.Events.ToList();
                const int batchSize = 10; // Reduced for time constraints

                _logger.LogInformation("Starting processing of {EventCount} events in batches of {BatchSize}", events.Count, batchSize);

                for (int i = 0; i < events.Count; i += batchSize)
                {
                    cts.Token.ThrowIfCancellationRequested();

                    var batch = events.Skip(i).Take(batchSize);
                    var processingResult = await _eventProcessingService.ProcessAndTagEventsAsync(batch);

                    _logger.LogInformation("Batch {BatchNumber}: Created {Created}, Updated {Updated}, Skipped {Skipped}",
                        (i / batchSize) + 1, processingResult.EventsCreated, processingResult.EventsUpdated, processingResult.EventsSkipped);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Function execution was cancelled due to timeout");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during event crawling process");
            throw; // Re-throw to trigger Azure Function retry policy
        }

        _logger.LogInformation("Event crawler function completed at: {Time}", DateTime.UtcNow);
    }


    [Function("CrawlAllEventsManual")]
    public async Task<HttpResponseData> CrawlAllEventsManual(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "crawl-all")] HttpRequestData req)
    {
        _logger.LogInformation("Manual crawl all sources requested");

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(8));

            _logger.LogInformation("Event crawler function started at: {Time}", DateTime.UtcNow);

            var crawlResult = await _crawlerService.CrawlAllSourcesAsync();
            _logger.LogInformation("Parallel crawling completed in {Duration}. Found {EventCount} events",
                crawlResult.Duration, crawlResult.EventsFound);

            var totalProcessed = 0;
            var totalCreated = 0;
            var totalUpdated = 0;
            var totalSkipped = 0;

            if (crawlResult.Success && crawlResult.Events.Any())
            {
                var events = crawlResult.Events.ToList();
                const int batchSize = 10;

                _logger.LogInformation("Starting processing of {EventCount} events in batches of {BatchSize}", events.Count, batchSize);

                for (int i = 0; i < events.Count; i += batchSize)
                {
                    cts.Token.ThrowIfCancellationRequested();

                    var batch = events.Skip(i).Take(batchSize);
                    var processingResult = await _eventProcessingService.ProcessAndTagEventsAsync(batch);

                    totalCreated += processingResult.EventsCreated;
                    totalUpdated += processingResult.EventsUpdated;
                    totalSkipped += processingResult.EventsSkipped;
                    totalProcessed += processingResult.EventsProcessed;

                    _logger.LogInformation("Batch {BatchNumber}: Created {Created}, Updated {Updated}, Skipped {Skipped}",
                        (i / batchSize) + 1, processingResult.EventsCreated, processingResult.EventsUpdated, processingResult.EventsSkipped);
                }
            }

            _logger.LogInformation("Event crawler function completed at: {Time}", DateTime.UtcNow);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "Crawl completed successfully",
                crawlDuration = crawlResult.Duration,
                eventsFound = crawlResult.EventsFound,
                eventsProcessed = totalProcessed,
                eventsCreated = totalCreated,
                eventsUpdated = totalUpdated,
                eventsSkipped = totalSkipped,
                startTime = DateTime.UtcNow.Subtract(crawlResult.Duration),
                endTime = DateTime.UtcNow
            });
            return response;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Function execution was cancelled due to timeout");

            var timeoutResponse = req.CreateResponse(HttpStatusCode.RequestTimeout);
            await timeoutResponse.WriteAsJsonAsync(new
            {
                success = false,
                error = "Function execution was cancelled due to timeout"
            });
            return timeoutResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual crawl all sources");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new
            {
                success = false,
                error = ex.Message,
                stackTrace = ex.StackTrace
            });
            return errorResponse;
        }
    }

    [Function("CrawlSpecificSourceFunction")]
    public async Task<HttpResponseData> CrawlSpecificSource(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "crawl/{source}")] HttpRequestData req, 
        string source)
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

    [Function("HealthCheckFunction")]
    public async Task<HttpResponseData> HealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            functions = new[] { "CrawlEventsFunction", "CrawlSpecificSourceFunction", "CrawlAllEventsManual" }
        });
        return response;
    }

    [Function("DiagnosticFunction")]
    public async Task<HttpResponseData> DiagnosticCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "diagnostic")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);

        try
        {
            var result = new
            {
                status = "startup_successful",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                servicesInjected = new
                {
                    crawlerService = _crawlerService != null,
                    eventProcessingService = _eventProcessingService != null,
                    logger = _logger != null
                },
                serviceTypes = new
                {
                    crawlerServiceType = _crawlerService?.GetType().Name ?? "null",
                    eventProcessingServiceType = _eventProcessingService?.GetType().Name ?? "null"
                },
                message = "Function app started successfully. Services are available."
            };

            await response.WriteAsJsonAsync(result);
        }
        catch (Exception ex)
        {
            await response.WriteAsJsonAsync(new
            {
                status = "diagnostic_error",
                error = ex.Message,
                message = "Error in diagnostic function"
            });
        }

        return response;
    }

    [Function("PingFunction")]
    public async Task<HttpResponseData> Ping([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("OK");
        return response;
    }
}
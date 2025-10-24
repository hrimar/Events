using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Crawler.Services.Implementations;
using Events.Crawler.Services.Interfaces;
using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Events.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        try
        {
            // Try multiple connection string formats for Azure compatibility
            var connectionString = context.Configuration.GetConnectionString("EventsConnection")
                ?? context.Configuration["ConnectionStrings__EventsConnection"];

            Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");
            Console.WriteLine($"Connection string found: {!string.IsNullOrEmpty(connectionString)}");

            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Configuring services with deferred database initialization...");

                // Configure DbContext with deferred connection and shorter timeout
                services.AddDbContext<EventsDbContext>(options =>
                {
                    options.UseSqlServer(connectionString, dbOptions =>
                    {
                        dbOptions.MigrationsAssembly("Events.Data");
                        dbOptions.EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                        dbOptions.CommandTimeout(30); // Shorter timeout
                    });

                    // Don't validate connection during startup
                    options.EnableServiceProviderCaching(false);
                    options.EnableSensitiveDataLogging(false);
                });

                // Core repositories
                services.AddScoped<IEventRepository, EventRepository>();
                services.AddScoped<ITagRepository, TagRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

                // Core services
                services.AddScoped<IEventService, EventService>();
                services.AddScoped<ISubCategoryService, SubCategoryService>();
                services.AddScoped<ITagService, TagService>();

                // Crawler-specific services with timeout protection
                services.AddScoped<ICrawlerService, SafeCrawlerService>();
                services.AddScoped<IEventProcessingService, SafeEventProcessingService>();
                services.AddScoped<IAiTaggingService, GroqTaggingService>();

                // HTTP clients with timeout configuration
                services.AddHttpClient<BiletBgCrawler>(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                // Crawler strategies - these will be resolved lazily
                services.AddScoped<IEventCrawlerStrategy, BiletBgCrawler>(); // via HttpClient
                services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EpaygoCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EventimCrawler>(); // via Playwright & AJAX

                Console.WriteLine("Services configured with database connection");
            }
            else
            {
                Console.WriteLine("No connection string found, configuring stub services...");

                // Register stub services
                services.AddSingleton<ICrawlerService, StubCrawlerService>();
                services.AddSingleton<IEventProcessingService, StubEventProcessingService>();

                Console.WriteLine("Stub services configured");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Service configuration error: {ex.Message}");

            // Register minimal stub services
            services.AddSingleton<ICrawlerService, StubCrawlerService>();
            services.AddSingleton<IEventProcessingService, StubEventProcessingService>();
        }
    })
    .Build();

host.Run();

// Safe wrapper for CrawlerService that handles initialization timeouts
public class SafeCrawlerService : ICrawlerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SafeCrawlerService> _logger;

    public SafeCrawlerService(IServiceProvider serviceProvider, ILogger<SafeCrawlerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<CrawlResult> CrawlEventsAsync(string source, DateTime? targetDate = null)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<CrawlerService>();

            if (actualService != null)
            {
                return await actualService.CrawlEventsAsync(source, targetDate);
            }

            return CreateErrorResult(source, "CrawlerService not available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeCrawlerService.CrawlEventsAsync");
            return CreateErrorResult(source, $"Service error: {ex.Message}");
        }
    }

    public async Task<CrawlResult> CrawlAllSourcesAsync(DateTime? targetDate = null)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<CrawlerService>();

            if (actualService != null)
            {
                return await actualService.CrawlAllSourcesAsync(targetDate);
            }

            return CreateErrorResult("all", "CrawlerService not available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeCrawlerService.CrawlAllSourcesAsync");
            return CreateErrorResult("all", $"Service error: {ex.Message}");
        }
    }

    public IEnumerable<string> GetSupportedSources()
    {
        return new[] { "bilet.bg", "ticketstation.bg", "eventim.bg", "epaygo.bg" };
    }

    private static CrawlResult CreateErrorResult(string source, string errorMessage)
    {
        return new CrawlResult
        {
            Source = source,
            Success = false,
            ErrorMessage = errorMessage,
            Events = new List<CrawledEventDto>(),
            CrawledAt = DateTime.UtcNow,
            Duration = TimeSpan.Zero
        };
    }
}

// Safe wrapper for EventProcessingService
public class SafeEventProcessingService : IEventProcessingService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SafeEventProcessingService> _logger;

    public SafeEventProcessingService(IServiceProvider serviceProvider, ILogger<SafeEventProcessingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<ProcessingResult> ProcessCrawledEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<EventProcessingService>();

            if (actualService != null)
            {
                return await actualService.ProcessCrawledEventsAsync(crawledEvents);
            }

            return CreateErrorResult("EventProcessingService not available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeEventProcessingService.ProcessCrawledEventsAsync");
            return CreateErrorResult($"Service error: {ex.Message}");
        }
    }

    public async Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<EventProcessingService>();

            if (actualService != null)
            {
                return await actualService.ProcessAndTagEventsAsync(crawledEvents);
            }

            return CreateErrorResult("EventProcessingService not available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeEventProcessingService.ProcessAndTagEventsAsync");
            return CreateErrorResult($"Service error: {ex.Message}");
        }
    }

    public async Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<EventProcessingService>();
            return actualService != null ? await actualService.FindExistingEventAsync(crawledEvent) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeEventProcessingService.FindExistingEventAsync");
            return null;
        }
    }

    public async Task<Event> MapToEntityAsync(CrawledEventDto crawledEvent)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var actualService = scope.ServiceProvider.GetService<EventProcessingService>();

            if (actualService != null)
            {
                return await actualService.MapToEntityAsync(crawledEvent);
            }

            throw new InvalidOperationException("EventProcessingService not available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SafeEventProcessingService.MapToEntityAsync");
            throw new InvalidOperationException($"Service error: {ex.Message}", ex);
        }
    }

    private static ProcessingResult CreateErrorResult(string errorMessage)
    {
        return new ProcessingResult
        {
            EventsProcessed = 0,
            EventsCreated = 0,
            EventsUpdated = 0,
            EventsSkipped = 0,
            Errors = new List<string> { errorMessage }
        };
    }
}

// Original stub services for when no connection string is available
public class StubCrawlerService : ICrawlerService
{
    public Task<CrawlResult> CrawlEventsAsync(string source, DateTime? targetDate = null)
    {
        return Task.FromResult(new CrawlResult
        {
            Source = source,
            Success = false,
            ErrorMessage = "Database connection not configured. Add 'EventsConnection' to Connection Strings with type 'SQLServer'.",
            Events = new List<CrawledEventDto>(),
            CrawledAt = DateTime.UtcNow,
            Duration = TimeSpan.Zero
        });
    }

    public Task<CrawlResult> CrawlAllSourcesAsync(DateTime? targetDate = null)
    {
        return Task.FromResult(new CrawlResult
        {
            Source = "all",
            Success = false,
            ErrorMessage = "Database connection not configured. Add 'EventsConnection' to Connection Strings with type 'SQLServer'.",
            Events = new List<CrawledEventDto>(),
            CrawledAt = DateTime.UtcNow,
            Duration = TimeSpan.Zero
        });
    }

    public IEnumerable<string> GetSupportedSources()
    {
        return new[] { "bilet.bg", "ticketstation.bg", "eventim.bg", "epaygo.bg" };
    }
}

public class StubEventProcessingService : IEventProcessingService
{
    public Task<ProcessingResult> ProcessCrawledEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        return Task.FromResult(new ProcessingResult
        {
            EventsProcessed = 0,
            EventsCreated = 0,
            EventsUpdated = 0,
            EventsSkipped = 0,
            Errors = new List<string> { "Database connection not configured" }
        });
    }

    public Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        return Task.FromResult(new ProcessingResult
        {
            EventsProcessed = 0,
            EventsCreated = 0,
            EventsUpdated = 0,
            EventsSkipped = 0,
            Errors = new List<string> { "Database connection not configured" }
        });
    }

    public Task<Event?> FindExistingEventAsync(CrawledEventDto crawledEvent)
    {
        return Task.FromResult<Event?>(null);
    }

    public Task<Event> MapToEntityAsync(CrawledEventDto crawledEvent)
    {
        throw new InvalidOperationException("Database connection not configured");
    }
}
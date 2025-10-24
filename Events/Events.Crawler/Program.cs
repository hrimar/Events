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
                ?? context.Configuration["ConnectionStrings__EventsConnection"]
                ?? context.Configuration["ConnectionStrings:EventsConnection"];

            var isDevelopment = context.HostingEnvironment.IsDevelopment();

            Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");
            Console.WriteLine($"Connection string found: {!string.IsNullOrEmpty(connectionString)}");

            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Configuring full services with database...");

                services.AddDbContext<EventsDbContext>(options =>
                    options.UseSqlServer(connectionString, dbOptions =>
                        dbOptions.MigrationsAssembly("Events.Data")));

                // Core repositories
                services.AddScoped<IEventRepository, EventRepository>();
                services.AddScoped<ITagRepository, TagRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

                // Core services
                services.AddScoped<IEventService, EventService>();
                services.AddScoped<ISubCategoryService, SubCategoryService>();
                services.AddScoped<ITagService, TagService>();

                // Crawler-specific services
                services.AddScoped<ICrawlerService, CrawlerService>();
                services.AddScoped<IEventProcessingService, EventProcessingService>();
                services.AddScoped<IAiTaggingService, GroqTaggingService>();

                // HTTP clients
                services.AddHttpClient<BiletBgCrawler>();

                // Crawler strategies
                services.AddScoped<IEventCrawlerStrategy, BiletBgCrawler>(); // via HttpClient
                services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EpaygoCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EventimCrawler>(); // via Playwright & AJAX

                Console.WriteLine("Full services configured successfully");
            }
            else
            {
                Console.WriteLine("No connection string found, configuring stub services...");

                // Create stub implementations that return meaningful errors
                services.AddScoped<ICrawlerService, StubCrawlerService>();
                services.AddScoped<IEventProcessingService, StubEventProcessingService>();

                Console.WriteLine("Stub services configured");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Service configuration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Register error-reporting services
            services.AddScoped<ICrawlerService, StubCrawlerService>();
            services.AddScoped<IEventProcessingService, StubEventProcessingService>();
        }
    })
    .Build();

host.Run();


public class StubCrawlerService : ICrawlerService
{
    public Task<CrawlResult> CrawlEventsAsync(string source, DateTime? targetDate = null)
    {
        return Task.FromResult(new CrawlResult
        {
            Source = source,
            Success = false,
            ErrorMessage = "Database connection not configured. Please add 'ConnectionStrings:EventsConnection' to application settings.",
            Events = new List<CrawledEventDto>()
        });
    }

    public Task<CrawlResult> CrawlAllSourcesAsync(DateTime? targetDate = null)
    {
        return Task.FromResult(new CrawlResult
        {
            Source = "all",
            Success = false,
            ErrorMessage = "Database connection not configured. Please add 'ConnectionStrings:EventsConnection' to application settings.",
            Events = new List<CrawledEventDto>()
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
            Errors = new List<string> { "Database connection not configured" }
        });
    }

    public Task<ProcessingResult> ProcessAndTagEventsAsync(IEnumerable<CrawledEventDto> crawledEvents)
    {
        return Task.FromResult(new ProcessingResult
        {
            EventsProcessed = 0,
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
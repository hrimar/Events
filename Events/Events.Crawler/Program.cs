using Events.Crawler.Services.Implementations;
using Events.Crawler.Services.Interfaces;
using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Services.Implementations;
using Events.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        try
        {
            // Database - use try-catch to handle missing connection string gracefully
            var connectionString = context.Configuration.GetConnectionString("EventsConnection");

            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<EventsDbContext>(options =>
                    options.UseSqlServer(connectionString, dbOptions =>
                        dbOptions.MigrationsAssembly("Events.Data")));

                // Core repositories (shared with web app)
                services.AddScoped<IEventRepository, EventRepository>();
                services.AddScoped<ITagRepository, TagRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

                // Core services (shared with web app)
                services.AddScoped<IEventService, EventService>();
                services.AddScoped<ISubCategoryService, SubCategoryService>();
                services.AddScoped<ITagService, TagService>();

                // Crawler-specific services
                services.AddScoped<ICrawlerService, CrawlerService>();
                services.AddScoped<IEventProcessingService, EventProcessingService>();
                services.AddScoped<IAiTaggingService, GroqTaggingService>();

                // HTTP clients for API crawlers
                services.AddHttpClient<BiletBgCrawler>();

                // Register crawler strategies
                services.AddScoped<IEventCrawlerStrategy, BiletBgCrawler>(); // via HttpClient
                services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EpaygoCrawler>(); // via Playwright
                services.AddScoped<IEventCrawlerStrategy, EventimCrawler>(); // via Playwright & AJAX
            }
            else
            {
                // Minimal services when connection string is not available
                // This allows deployment to succeed even without database configuration
                services.AddScoped<ICrawlerService>(provider =>
                    throw new InvalidOperationException("Database connection not configured"));
                services.AddScoped<IEventProcessingService>(provider =>
                    throw new InvalidOperationException("Database connection not configured"));
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the startup - this allows deployment to succeed
            Console.WriteLine($"Warning: Service configuration failed: {ex.Message}");

            // Register minimal stub services to prevent DI failures
            services.AddScoped<ICrawlerService>(provider =>
                throw new InvalidOperationException($"Service configuration error: {ex.Message}"));
            services.AddScoped<IEventProcessingService>(provider =>
                throw new InvalidOperationException($"Service configuration error: {ex.Message}"));
        }
    })
    .Build();

host.Run();
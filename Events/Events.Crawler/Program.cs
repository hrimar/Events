using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
using Events.Crawler.Services.Implementations;
using Events.Crawler.Services.Interfaces;
using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Services.Caching;
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

        // Azure Functions isolated host in a container reads configuration exclusively from environment variables.
        // GetConnectionString() maps to the ConnectionStrings__<name> env var automatically via the .NET config system.
        var connectionString =
            context.Configuration.GetConnectionString("EventsConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__EventsConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection not configured. Set 'ConnectionStrings__EventsConnection' environment variable in docker-compose.yml or Azure App Settings.");
        }

        Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");

        // Configure DbContext
        services.AddDbContext<EventsDbContext>(options =>
        {
            options.UseSqlServer(connectionString, dbOptions =>
            {
                dbOptions.MigrationsAssembly("Events.Data");
                dbOptions.EnableRetryOnFailure(
                    maxRetryCount: 2,  // reduced retries
                    maxRetryDelay: TimeSpan.FromSeconds(5),  // reduced delay
                    errorNumbersToAdd: null);
                dbOptions.CommandTimeout(30);  // reduced timeout
            });
        }, ServiceLifetime.Scoped);  // Explicit scoped lifetime

        // Core repositories
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IEventTagRepository, EventTagRepository>(); // Bulk operations repository
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();

        // Core services
        // EventService/TagService cache Event/Tag query results using these - not that this crawler
        // process benefits from the caching itself (it mostly writes, doesn't serve listing
        // requests), but their constructors require these dependencies regardless. Each crawler run
        // is its own process/DI container, so this cache never persists or interacts with Events.Web's.
        services.AddMemoryCache();
        services.AddSingleton<IEventCacheInvalidator, EventCacheInvalidator>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IVenueService, VenueService>();

        // Crawler services
        services.AddScoped<ICrawlerService, CrawlerService>();
        services.AddScoped<IEventProcessingService, EventProcessingService>();
        services.AddScoped<IAiTaggingService, ClaudeProcessingService>();

        // HTTP clients
        services.AddHttpClient<BiletBgApiCrawler>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient<PayseraApiCrawler>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Crawler strategies
        services.AddScoped<IEventCrawlerStrategy, BiletBgApiCrawler>();    // via HttpClient. Paginated JSON requests to API ~7 min for the first invocation locally
        services.AddScoped<IEventCrawlerStrategy, PayseraApiCrawler>();    // via HttpClient. Offset-paginated JSON requests to API ~0.5 min for the first invocation in production
        //services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>(); // via Playwright. 3 levels of deep click. Optimized ~7 min for the first invocation. Wworks only locally now!
        services.AddScoped<IEventCrawlerStrategy, EventimCrawler>();       // via Playwright & AJAX. No clicks ~7 min for the first invocation
        services.AddScoped<IEventCrawlerStrategy, EpaygoCrawler>();        // via Playwright. Two Phases ~10 min for the first invocation
        services.AddScoped<IEventCrawlerStrategy, NdkCrawler>();           // via Playwright. No clicks  ~0.5 min for the first invocation
        services.AddScoped<IEventCrawlerStrategy, EntaseCrawler>();        // via Playwright. 2 levels of deep click. ~1.8 min for the first invocation
        //services.AddScoped<IEventCrawlerStrategy, KupibiletiCrawler>();  // via Playwright & FlareSolverr Docker dependency. 3 levels of deep click ~14-17 min. Wworks only locally!

        System.Diagnostics.Trace.WriteLine("Services configured successfully");
    })
    .Build();

host.Run();
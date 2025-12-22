using Events.Crawler.DTOs.Common;
using Events.Crawler.Models;
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
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        var connectionString = context.Configuration.GetConnectionString("EventsConnection")
            ?? context.Configuration["ConnectionStrings__EventsConnection"];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection not configured. Add 'EventsConnection' to Connection Strings with type 'SQLServer'."); 
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

        // Core services
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<ITagService, TagService>();

        // Crawler services
        services.AddScoped<ICrawlerService, CrawlerService>();
        services.AddScoped<IEventProcessingService, EventProcessingService>();
        services.AddScoped<IAiTaggingService, GroqTaggingService>();

        // HTTP clients
        services.AddHttpClient<BiletBgApiCrawler>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Crawler strategies
        services.AddScoped<IEventCrawlerStrategy, BiletBgApiCrawler>(); // via HttpClient
        services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>(); // via Playwright
        //services.AddScoped<IEventCrawlerStrategy, EpaygoCrawler>(); // via Playwright
        //services.AddScoped<IEventCrawlerStrategy, EventimCrawler>(); // via Playwright & AJAX

        Console.WriteLine("Services configured successfully");
    })
    .Build();

host.Run();
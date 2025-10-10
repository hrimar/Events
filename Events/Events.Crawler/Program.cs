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
        // Database
        var connectionString = context.Configuration.GetConnectionString("EventsConnection")
            ?? throw new InvalidOperationException("Connection string 'EventsConnection' not found.");

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
        services.AddScoped<IEventCrawlerStrategy, BiletBgCrawler>();
        services.AddScoped<IEventCrawlerStrategy, TicketStationCrawler>();
    })
    .Build();

host.Run();

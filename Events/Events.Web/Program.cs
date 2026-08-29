using Azure.Identity;
using Azure.Storage.Blobs;
using Events.Data.Context;
using Events.Web.Infrastructure;
using Events.Web.Services;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Data.Services;
using Events.Models.Entities;
using Events.Services.Caching;
using Events.Services.Implementations;
using Events.Services.Import;
using Events.Services.Import.Parsers;
using Events.Services.Interfaces;
using Events.Web.Options;
using Events.Web.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Guarded, not called unconditionally: AddApplicationInsightsTelemetry() throws at startup
// ("A connection string was not found") when APPLICATIONINSIGHTS_CONNECTION_STRING is entirely
// absent from configuration, rather than no-op'ing - so it must only be registered when the
// value (set by Terraform in production) is actually present. Gives request telemetry,
// exceptions, and SQL dependency tracking (which endpoint/query is slow) with no other wiring.
var appInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
{
    builder.Services.AddApplicationInsightsTelemetry();
}

ConfigureDatabase(builder);
ConfigureIdentity(builder);
ConfigureAuthorization(builder);
ConfigureAzureStorage(builder);
ConfigureEmail(builder);
RegisterServices(builder);

ConfigureLocalization(builder);
ConfigureRateLimiting(builder);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(Events.Web.Resources.SharedResources));
    });
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (_, factory) =>
            factory.Create(typeof(Events.Web.Resources.SharedResources));
    })
    .AddMvcOptions(options =>
    {
        // Insert before the default decimal binder so dot-separated decimals (e.g. coordinates)
        // are parsed correctly regardless of the active request culture (bg uses comma by default).
        options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
    });
builder.Services.AddScoped<Events.Web.Localization.IdentityMessages>();

var app = builder.Build();

// Production-safe database initialization (no auto-migrations)
await InitializeDatabaseAsync(app);

ConfigureHttpPipeline(app);

app.Run();

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    // Support for design-time operations with environment variable fallback
    var connectionString = builder.Configuration.GetConnectionString("EventsConnection")
        ?? Environment.GetEnvironmentVariable("DESIGN_TIME_CONNECTION_STRING")
        ?? throw new InvalidOperationException("Connection string 'EventsConnection' not found.");

    builder.Services.AddDbContext<EventsDbContext>(options =>
    {
        options.UseSqlServer(connectionString, dbOptions =>
        {
            dbOptions.MigrationsAssembly("Events.Data");
            dbOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            dbOptions.CommandTimeout(120);
        });
    });

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }
}

static void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
    })
    .AddEntityFrameworkStores<EventsDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
}

static void ConfigureAuthorization(WebApplicationBuilder builder)
{
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrator", "EventManager"));
    });
}

static bool IsDesignTime(WebApplicationBuilder builder)
{
    return builder.Configuration["__DesignTime"] == "true" || string.IsNullOrEmpty(builder.Configuration.GetConnectionString("EventsConnection"));
}

static void ConfigureAzureStorage(WebApplicationBuilder builder)
{
    var blobStorageUri = builder.Configuration["BlobStorage:Uri"];

    // Allow null during design-time (EF migrations in CI/CD)
    // In production, App Service environment variables will provide the value
    if (!string.IsNullOrEmpty(blobStorageUri))
    {
        var containerUri = new Uri($"{blobStorageUri.TrimEnd('/')}/event-images");
        var blobContainerClient = new BlobContainerClient(containerUri, new DefaultAzureCredential());

        builder.Services.AddSingleton(blobContainerClient);
        builder.Services.AddScoped<IImageUploadService, AzureBlobImageService>();
    }
    else if (!builder.Environment.IsDevelopment() && !IsDesignTime(builder))
    {
        // Only throw in production, not during design-time migrations
        throw new InvalidOperationException("BlobStorage:Uri configuration is missing. Please configure it in App Service environment variables or appsettings.json");
    }
    // In development/design-time without BlobStorage: skip registration (won't be used for migrations)
}

static void ConfigureEmail(WebApplicationBuilder builder)
{
    const string sectionName = "Smtp";
    var smtpSection = builder.Configuration.GetSection(sectionName);

    builder.Services.AddOptions<SmtpOptions>()
        .Bind(smtpSection)
        .ValidateDataAnnotations()
        .Validate(o => !string.IsNullOrWhiteSpace(o.From), "Smtp:From is required.")
        .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "Smtp:Host is required.")
        .Validate(o => o.Port > 0, "Smtp:Port must be greater than zero.")
        .ValidateOnStart();

    builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
}

static void ConfigureLocalization(WebApplicationBuilder builder)
{
    builder.Services.AddLocalization(options => options.ResourcesPath = string.Empty);

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "bg", "en" };

        options.SetDefaultCulture("bg")
               .AddSupportedCultures(supportedCultures)
               .AddSupportedUICultures(supportedCultures);

        // Cookie provider is first - persists user language choice between sessions
        options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CookieRequestCultureProvider());
    });
}

static void ConfigureRateLimiting(WebApplicationBuilder builder)
{
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Logged + Retry-After header on any rejection, from any policy below - lets us see in logs who's actually
        // hitting these limits (IP, path), which is also the raw data a future bot/scraping-defense decision
        // would want before choosing a bigger tool (WAF, bot detection, etc.).
        // This app-level limiter is one layer, not the final anti-scraping answer - it only caps request frequency per IP,
        // and doesn't conflict with adding an edge-level defense (e.g. Azure Front Door + WAF)
        // in front of it later; that would simply filter traffic before it reaches here.
        options.OnRejected = (context, cancellationToken) =>
        {
            context.HttpContext.Response.Headers.RetryAfter = "60";

            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Rate limit exceeded: {Ip} on {Path}",
                context.HttpContext.Connection.RemoteIpAddress, context.HttpContext.Request.Path);

            return ValueTask.CompletedTask;
        };

        options.AddPolicy("contact", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

        // Token bucket, not fixed window: a fixed window can be gamed for ~2x its nominal rate
        // by timing requests around the window boundary (e.g. 30 requests at 0:59, another 30 at
        // 1:01). Token bucket avoids that - the bucket starts full (allows an initial burst), then
        // refills steadily, so the achievable rate stays close to the intended limit no matter how
        // requests are timed. Numbers are a starting point (no real traffic baseline yet - see
        // Phase 2 observability) and should be recalibrated once Application Insights data exists.
        options.AddPolicy("events", httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 40,
                    TokensPerPeriod = 30,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    AutoReplenishment = true,
                    QueueLimit = 0
                }));
    });
}

static void RegisterServices(WebApplicationBuilder builder)
{
    // Infrastructure
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ISiteUrlProvider, SiteUrlProvider>();
    builder.Services.AddSingleton<ContactFormTimingProtector>();
    // /health, checked by the App Service platform (health_check_path in Terraform) and available
    // for manual/external monitoring - verifies the app can actually reach the database.
    builder.Services.AddHealthChecks().AddDbContextCheck<EventsDbContext>();

    // Repositories
    builder.Services.AddScoped<IEventRepository, EventRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<IEventTagRepository, EventTagRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
    builder.Services.AddScoped<IUserFavoriteEventRepository, UserFavoriteEventRepository>();
    builder.Services.AddScoped<IVenueRepository, VenueRepository>();
    builder.Services.AddScoped<ISiteContentRepository, SiteContentRepository>();
    builder.Services.AddScoped<IPageSeoMetaRepository, PageSeoMetaRepository>();

    // Services
    // This IMemoryCache is shared - also used below by EventImportBatchCache for the bulk create from .xlsx/.csv admin feature.
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<IEventCacheInvalidator, EventCacheInvalidator>();
    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();
    builder.Services.AddScoped<IUserFavoriteEventService, UserFavoriteEventService>();
    builder.Services.AddScoped<IAdminUserService, AdminUserService>();
    builder.Services.AddScoped<IEventFilterOptionsBuilder, EventFilterOptionsBuilder>();
    builder.Services.AddScoped<IVenueService, VenueService>();
    builder.Services.AddScoped<ISiteContentService, SiteContentService>();
    builder.Services.AddScoped<ISeoMetaService, SeoMetaService>();

    // Event import (bulk create from .xlsx/.csv)
    builder.Services.AddScoped<IEventImportFileParser, XlsxEventImportParser>();
    builder.Services.AddScoped<IEventImportFileParser, CsvEventImportParser>();
    builder.Services.AddScoped<IEventImportFileParserFactory, EventImportFileParserFactory>();
    builder.Services.AddScoped<IEventImportRowMapper, EventImportRowMapper>();
    builder.Services.AddScoped<IEventImportDuplicateDetector, EventImportDuplicateDetector>();
    builder.Services.AddScoped<IEventImportService, EventImportService>();
    builder.Services.AddSingleton<Events.Web.Areas.Admin.Services.EventImportBatchCache>();
}

static async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Development: Full initialization with migrations
        if (app.Environment.IsDevelopment())
        {
            var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();

            logger.LogInformation("Development environment - applying migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed successfully");

            logger.LogInformation("Seeding roles and users...");
            await DbSeederService.SeedDatabaseAsync(app.Services, logger);
        }
        else
        {
            // Production: Only seed roles (schema should be ready from pipeline)
            logger.LogInformation("Production environment - seeding roles only (no migrations)");
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await DbSeederService.SeedRolesAsync(roleManager, logger);
            logger.LogInformation("Production roles seeded - manual user creation required");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing database");
        throw;
    }
}

static void ConfigureHttpPipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Security headers for all environments
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        if (!app.Environment.IsDevelopment())
        {
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        await next();
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRequestLocalization();
    app.UseRouting();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");

    app.MapControllerRoute(
        name: "sitemap",
        pattern: "sitemap.xml",
        defaults: new { controller = "Seo", action = "Sitemap" });

    app.MapControllerRoute(
        name: "robots",
        pattern: "robots.txt",
        defaults: new { controller = "Seo", action = "Robots" });

    app.MapControllerRoute(
        name: "venue-details",
        pattern: "venues/{slug}",
        defaults: new { controller = "Venues", action = "Details" });

    app.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();
}

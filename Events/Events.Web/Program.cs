using Events.Data.Context;
using Events.Data.Services;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Events.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder);
ConfigureIdentity(builder);
ConfigureAuthorization(builder);
ConfigureAzureStorage(builder);
RegisterServices(builder);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

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

static void ConfigureAzureStorage(WebApplicationBuilder builder)
{
    var blobStorageUri = builder.Configuration["BlobStorage:Uri"];

    if (!string.IsNullOrEmpty(blobStorageUri))
    {
        var containerUri = new Uri($"{blobStorageUri.TrimEnd('/')}/event-images");
        var blobContainerClient = new BlobContainerClient(containerUri, new DefaultAzureCredential());

        builder.Services.AddSingleton(blobContainerClient);
        builder.Services.AddScoped<IImageUploadService, AzureBlobImageService>();
    }
    else
    {
        throw new InvalidOperationException("BlobStorage:Uri configuration is missing. Please configure it in appsettings.json");
    }
}

static void RegisterServices(WebApplicationBuilder builder)
{
    // Repositories
    builder.Services.AddScoped<IEventRepository, EventRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<IEventTagRepository, EventTagRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
    builder.Services.AddScoped<IUserFavoriteEventRepository, UserFavoriteEventRepository>();

    // Services
    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IUserFavoriteEventService, UserFavoriteEventService>();
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
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();
}

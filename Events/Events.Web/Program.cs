using Events.Data.Context;
using Events.Data.Services;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Services.Implementations;
using Events.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder);

ConfigureIdentity(builder);

ConfigureAuthorization(builder);

RegisterServices(builder);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Database initialization (Development only)
await InitializeDatabaseAsync(app);

ConfigureHttpPipeline(app);

app.Run();


static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("EventsConnection") ??
        throw new InvalidOperationException("Connection string 'EventsConnection' not found.");

    builder.Services.AddDbContext<EventsDbContext>(options =>
        options.UseSqlServer(connectionString, dbOptions =>
            dbOptions.MigrationsAssembly("Events.Data")));

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

static void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Disable email confirmation for development
        if (builder.Environment.IsDevelopment())
        {
            options.SignIn.RequireConfirmedAccount = false;
        }
        else
        {
            options.SignIn.RequireConfirmedAccount = true;
        }
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

static void RegisterServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IEventRepository, EventRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<ITagService, TagService>();
}

static async Task InitializeDatabaseAsync(WebApplication app)
{
    if (!app.Environment.IsDevelopment()) return;

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Ensuring database exists and applying migrations...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations completed successfully");

        logger.LogInformation("Starting database seeding...");
        await DbSeederService.SeedDatabaseAsync(app.Services, logger);
        logger.LogInformation("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database");
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

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Admin Area route
    app.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

    // Default route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    app.MapRazorPages();
}

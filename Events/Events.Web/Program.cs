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
    //    builder.Services.AddDefaultIdentity<IdentityUser>(options => // this doesn't work with roles
    //{
    //        options.SignIn.RequireConfirmedAccount = true;

    //        // Uncomment for development with relaxed password requirements
    //        // if (builder.Environment.IsDevelopment())
    //        // {
    //        //     options.Password.RequireDigit = true;
    //        //     options.Password.RequireLowercase = true;
    //        //     options.Password.RequireNonAlphanumeric = false;
    //        //     options.Password.RequireUppercase = false;
    //        //     options.Password.RequiredLength = 6;
    //        //     options.Password.RequiredUniqueChars = 1;
    //        // }
    //    })
    //        .AddRoles<IdentityRole>()
    //        .AddEntityFrameworkStores<EventsDbContext>();
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

static void RegisterServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IEventRepository, EventRepository>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

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

    // Route configuration
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();
}

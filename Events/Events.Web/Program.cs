using Events.Data.Context;
using Events.Data.Services;
using Events.Data.Repositories.Implementations;
using Events.Data.Repositories.Interfaces;
using Events.Services.Implementations;
using Events.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("EventsConnection") ??
    throw new InvalidOperationException("Connection string 'EventsConnection' not found.");

builder.Services.AddDbContext<EventsDbContext>(options =>
    options.UseSqlServer(connectionString, dbOptions =>
        dbOptions.MigrationsAssembly("Events.Data")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;

    //if (builder.Environment.IsDevelopment()) // for simpler registration and operation with User logins
    //{
        //    options.Password.RequireDigit = true;
        //    options.Password.RequireLowercase = true;
        //    options.Password.RequireNonAlphanumeric = false; // true;
        //    options.Password.RequireUppercase = false; // true;
        //    options.Password.RequiredLength = 6;
        //    options.Password.RequiredUniqueChars = 1;
    //}
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<EventsDbContext>();

// Register repositories
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Register services
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Migrate and seed the database in Development only. Re-throw in development to see the issue
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
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
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

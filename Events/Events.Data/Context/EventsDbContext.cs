using Events.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Context;

public class EventsDbContext : IdentityDbContext
{
    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<EventTag> EventTags { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Event entity
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.TicketUrl).HasMaxLength(500);
            entity.Property(e => e.SourceUrl).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.Property(e => e.Category).IsRequired(false);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Status);
        });

        // Configure Tag entity
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Description).HasMaxLength(500);

            entity.HasIndex(t => t.Name).IsUnique();
        });

        // Configure EventTag many-to-many relationship
        modelBuilder.Entity<EventTag>(entity =>
        {
            entity.HasKey(et => new { et.EventId, et.TagId });

            entity.HasOne(et => et.Event)
                .WithMany(e => e.EventTags)
                .HasForeignKey(et => et.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                .WithMany(t => t.EventTags)
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.DefaultImageUrl).HasMaxLength(500);

            entity.HasIndex(c => c.CategoryType);
        });

        // Seed initial categories
        SeedCategories(modelBuilder);
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Music", CategoryType = Models.Enums.EventCategory.Music, Description = "Musical events and concerts" },
            new Category { Id = 2, Name = "Art", CategoryType = Models.Enums.EventCategory.Art, Description = "Art exhibitions and shows" },
            new Category { Id = 3, Name = "Business", CategoryType = Models.Enums.EventCategory.Business, Description = "Business conferences and networking" },
            new Category { Id = 4, Name = "Sports", CategoryType = Models.Enums.EventCategory.Sports, Description = "Sports events and competitions" },
            new Category { Id = 5, Name = "Theatre", CategoryType = Models.Enums.EventCategory.Theatre, Description = "Theatre performances and plays" },
            new Category { Id = 6, Name = "Cinema", CategoryType = Models.Enums.EventCategory.Cinema, Description = "Movie screenings and film festivals" },
            new Category { Id = 7, Name = "Festivals", CategoryType = Models.Enums.EventCategory.Festivals, Description = "Various festivals and celebrations" },
            new Category { Id = 8, Name = "Exhibitions", CategoryType = Models.Enums.EventCategory.Exhibitions, Description = "Exhibitions and displays" },
            new Category { Id = 9, Name = "Conferences", CategoryType = Models.Enums.EventCategory.Conferences, Description = "Professional conferences and seminars" },
            new Category { Id = 10, Name = "Workshops", CategoryType = Models.Enums.EventCategory.Workshops, Description = "Educational workshops and training" }
        );
    }
}
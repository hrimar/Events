using Events.Models.Entities;
using Events.Models.Enums;
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
    public DbSet<SubCategory> SubCategories { get; set; }

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

            // Configure Category relationship
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.Status);
        });

        // Configure SubCategory entity
        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.Name).IsRequired().HasMaxLength(100);
            entity.Property(sc => sc.Description).HasMaxLength(500);

            entity.HasIndex(sc => new { sc.CategoryId, sc.EnumValue }).IsUnique();
            entity.HasIndex(sc => sc.CategoryId);

            // Set database default for CreatedAt
            entity.Property(sc => sc.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure Category relationship
            entity.HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        ConfigureTagEntity(modelBuilder);
        ConfigureEventTagEntity(modelBuilder);
        ConfigureCategoryEntity(modelBuilder);

        SeedCategories(modelBuilder);
        SeedSubCategories(modelBuilder);
    }

    private static void SeedSubCategories(ModelBuilder modelBuilder)
    {
        var subCategories = new List<SubCategory>();
        int id = 1;

        // Music subcategories (CategoryId = 1)
        foreach (MusicSubCategory musicSub in Enum.GetValues<MusicSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = musicSub.ToString(),
                Description = GetMusicSubCategoryDescription(musicSub),
                ParentCategory = EventCategory.Music,
                CategoryId = 1, // Music Category
                EnumValue = (int)musicSub
            });
        }

        // Sports subcategories (CategoryId = 4)
        foreach (SportsSubCategory sportsSub in Enum.GetValues<SportsSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = sportsSub.ToString(),
                Description = GetSportsSubCategoryDescription(sportsSub),
                ParentCategory = EventCategory.Sports,
                CategoryId = 4, // Sports Category
                EnumValue = (int)sportsSub
            });
        }

        modelBuilder.Entity<SubCategory>().HasData(subCategories);
    }

    private static string GetMusicSubCategoryDescription(MusicSubCategory category)
    {
        return category switch
        {
            MusicSubCategory.Rock => "Rock music events and concerts",
            MusicSubCategory.Jazz => "Jazz performances and sessions",
            MusicSubCategory.Metal => "Heavy metal and metal subgenres",
            MusicSubCategory.Pop => "Pop music events",
            MusicSubCategory.Classical => "Classical music concerts",
            MusicSubCategory.Electronic => "Electronic music and DJ sets",
            MusicSubCategory.Folk => "Folk and traditional music",
            MusicSubCategory.Blues => "Blues music performances",
            MusicSubCategory.HipHop => "Hip-hop and rap events",
            _ => $"{category} music events"
        };
    }

    private static string GetSportsSubCategoryDescription(SportsSubCategory category)
    {
        return category switch
        {
            SportsSubCategory.Football => "Football matches and events",
            SportsSubCategory.Basketball => "Basketball games and tournaments",
            SportsSubCategory.Tennis => "Tennis matches and tournaments",
            SportsSubCategory.Volleyball => "Volleyball games",
            SportsSubCategory.Swimming => "Swimming competitions",
            SportsSubCategory.Athletics => "Track and field events",
            _ => $"{category} events and competitions"
        };
    }

    private static void ConfigureTagEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.HasIndex(t => t.Name).IsUnique();
        });
    }

    private static void ConfigureEventTagEntity(ModelBuilder modelBuilder)
    {
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
    }

    private static void ConfigureCategoryEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.DefaultImageUrl).HasMaxLength(500);
            entity.HasIndex(c => c.CategoryType).IsUnique();
            
            // Set database default for CreatedAt
            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }

    private static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Music", CategoryType = EventCategory.Music, Description = "Musical events and concerts" },
            new Category { Id = 2, Name = "Art", CategoryType = EventCategory.Art, Description = "Art exhibitions and shows" },
            new Category { Id = 3, Name = "Business", CategoryType = EventCategory.Business, Description = "Business conferences and networking" },
            new Category { Id = 4, Name = "Sports", CategoryType = EventCategory.Sports, Description = "Sports events and competitions" },
            new Category { Id = 5, Name = "Theatre", CategoryType = EventCategory.Theatre, Description = "Theatre performances and plays" },
            new Category { Id = 6, Name = "Cinema", CategoryType = EventCategory.Cinema, Description = "Movie screenings and film festivals" },
            new Category { Id = 7, Name = "Festivals", CategoryType = EventCategory.Festivals, Description = "Various festivals and celebrations" },
            new Category { Id = 8, Name = "Exhibitions", CategoryType = EventCategory.Exhibitions, Description = "Exhibitions and displays" },
            new Category { Id = 9, Name = "Conferences", CategoryType = EventCategory.Conferences, Description = "Professional conferences and seminars" },
            new Category { Id = 10, Name = "Workshops", CategoryType = EventCategory.Workshops, Description = "Educational workshops and training" },
            new Category { Id = 11, Name = "Undefined", CategoryType = EventCategory.Undefined, Description = "Events pending categorization" }
        );
    }
}
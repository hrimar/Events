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

            // Configure SubCategory relationship
            entity.HasOne(e => e.SubCategory)
                .WithMany(sc => sc.Events)
                .HasForeignKey(e => e.SubCategoryId)
                .OnDelete(DeleteBehavior.SetNull); // If SubCategory is deleted, the Events stays and SubCategoryId is set to null

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.SubCategoryId);
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
                CategoryId = 1,
                EnumValue = (int)musicSub
            });
        }

        // Art subcategories (CategoryId = 2)
        foreach (ArtSubCategory artSub in Enum.GetValues<ArtSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = artSub.ToString(),
                Description = GetArtSubCategoryDescription(artSub),
                ParentCategory = EventCategory.Art,
                CategoryId = 2,
                EnumValue = (int)artSub
            });
        }

        // Business subcategories (CategoryId = 3)
        foreach (BusinessSubCategory businessSub in Enum.GetValues<BusinessSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = businessSub.ToString(),
                Description = GetBusinessSubCategoryDescription(businessSub),
                ParentCategory = EventCategory.Business,
                CategoryId = 3,
                EnumValue = (int)businessSub
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
                CategoryId = 4,
                EnumValue = (int)sportsSub
            });
        }

        // Theatre subcategories (CategoryId = 5)
        foreach (TheatreSubCategory theatreSub in Enum.GetValues<TheatreSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = theatreSub.ToString(),
                Description = GetTheatreSubCategoryDescription(theatreSub),
                ParentCategory = EventCategory.Theatre,
                CategoryId = 5,
                EnumValue = (int)theatreSub
            });
        }

        // Cinema subcategories (CategoryId = 6)
        foreach (CinemaSubCategory cinemaSub in Enum.GetValues<CinemaSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = cinemaSub.ToString(),
                Description = GetCinemaSubCategoryDescription(cinemaSub),
                ParentCategory = EventCategory.Cinema,
                CategoryId = 6,
                EnumValue = (int)cinemaSub
            });
        }

        // Festivals subcategories (CategoryId = 7)
        foreach (FestivalsSubCategory festivalsSub in Enum.GetValues<FestivalsSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = festivalsSub.ToString(),
                Description = GetFestivalsSubCategoryDescription(festivalsSub),
                ParentCategory = EventCategory.Festivals,
                CategoryId = 7,
                EnumValue = (int)festivalsSub
            });
        }

        // Exhibitions subcategories (CategoryId = 8)
        foreach (ExhibitionsSubCategory exhibitionsSub in Enum.GetValues<ExhibitionsSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = exhibitionsSub.ToString(),
                Description = GetExhibitionsSubCategoryDescription(exhibitionsSub),
                ParentCategory = EventCategory.Exhibitions,
                CategoryId = 8,
                EnumValue = (int)exhibitionsSub
            });
        }

        // Conferences subcategories (CategoryId = 9)
        foreach (ConferencesSubCategory conferencesSub in Enum.GetValues<ConferencesSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = conferencesSub.ToString(),
                Description = GetConferencesSubCategoryDescription(conferencesSub),
                ParentCategory = EventCategory.Conferences,
                CategoryId = 9,
                EnumValue = (int)conferencesSub
            });
        }

        // Workshops subcategories (CategoryId = 10)
        foreach (WorkshopsSubCategory workshopsSub in Enum.GetValues<WorkshopsSubCategory>())
        {
            subCategories.Add(new SubCategory
            {
                Id = id++,
                Name = workshopsSub.ToString(),
                Description = GetWorkshopsSubCategoryDescription(workshopsSub),
                ParentCategory = EventCategory.Workshops,
                CategoryId = 10,
                EnumValue = (int)workshopsSub
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
            MusicSubCategory.Other => "Other music events",
            _ => $"{category} music events"
        };
    }

    private static string GetArtSubCategoryDescription(ArtSubCategory category)
    {
        return category switch
        {
            ArtSubCategory.Painting => "Painting exhibitions and shows",
            ArtSubCategory.Sculpture => "Sculpture exhibitions",
            ArtSubCategory.Photography => "Photography exhibitions",
            ArtSubCategory.DigitalArt => "Digital art exhibitions",
            ArtSubCategory.StreetArt => "Street art and urban art events",
            ArtSubCategory.Graffiti => "Graffiti art exhibitions",
            ArtSubCategory.Illustration => "Illustration exhibitions",
            ArtSubCategory.PerformanceArt => "Performance art events",
            ArtSubCategory.InstallationArt => "Installation art exhibitions",
            ArtSubCategory.ContemporaryArt => "Contemporary art exhibitions",
            ArtSubCategory.VisualArts => "Visual arts events",
            ArtSubCategory.MixedMedia => "Mixed media art exhibitions",
            ArtSubCategory.ConceptualArt => "Conceptual art exhibitions",
            ArtSubCategory.Other => "Other art events",
            _ => $"{category} art events"
        };
    }

    private static string GetBusinessSubCategoryDescription(BusinessSubCategory category)
    {
        return category switch
        {
            BusinessSubCategory.NetworkingEvents => "Business networking events",
            BusinessSubCategory.Startups => "Startup events and pitches",
            BusinessSubCategory.Entrepreneurship => "Entrepreneurship seminars",
            BusinessSubCategory.Marketing => "Marketing conferences",
            BusinessSubCategory.Sales => "Sales training and events",
            BusinessSubCategory.Leadership => "Leadership development events",
            BusinessSubCategory.Finance => "Finance and investment events",
            BusinessSubCategory.RealEstate => "Real estate events",
            BusinessSubCategory.Investment => "Investment seminars",
            BusinessSubCategory.ECommerce => "E-commerce events",
            BusinessSubCategory.Innovation => "Innovation forums",
            BusinessSubCategory.Technology => "Technology business events",
            BusinessSubCategory.HRManagement => "HR and management events",
            BusinessSubCategory.BusinessStrategy => "Business strategy workshops",
            BusinessSubCategory.ProductDevelopment => "Product development events",
            BusinessSubCategory.Other => "Other business events",
            _ => $"{category} business events"
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
            SportsSubCategory.Other => "Other sports events and competitions",
            _ => $"{category} events and competitions"
        };
    }

    private static string GetTheatreSubCategoryDescription(TheatreSubCategory category)
    {
        return category switch
        {
            TheatreSubCategory.Drama => "Drama theatre performances",
            TheatreSubCategory.Comedy => "Comedy theatre shows",
            TheatreSubCategory.MusicalTheatre => "Musical theatre productions",
            TheatreSubCategory.Tragedy => "Tragedy theatre performances",
            TheatreSubCategory.ExperimentalTheatre => "Experimental theatre",
            TheatreSubCategory.PuppetTheatre => "Puppet theatre shows",
            TheatreSubCategory.Improvisation => "Improvisation theatre",
            TheatreSubCategory.StreetTheatre => "Street theatre performances",
            TheatreSubCategory.Monodrama => "Monodrama performances",
            TheatreSubCategory.ChildrensTheatre => "Children's theatre shows",
            TheatreSubCategory.Other => "Other theatre performances",
            _ => $"{category} theatre events"
        };
    }

    private static string GetCinemaSubCategoryDescription(CinemaSubCategory category)
    {
        return category switch
        {
            CinemaSubCategory.FeatureFilms => "Feature film screenings",
            CinemaSubCategory.ShortFilms => "Short film screenings",
            CinemaSubCategory.Documentaries => "Documentary screenings",
            CinemaSubCategory.Animation => "Animation film screenings",
            CinemaSubCategory.IndependentCinema => "Independent cinema screenings",
            CinemaSubCategory.BulgarianCinema => "Bulgarian cinema screenings",
            CinemaSubCategory.InternationalCinema => "International cinema screenings",
            CinemaSubCategory.FilmPremieres => "Film premiere events",
            CinemaSubCategory.StudentFilms => "Student film screenings",
            CinemaSubCategory.FilmFestivals => "Film festival events",
            CinemaSubCategory.Other => "Other cinema events",
            _ => $"{category} cinema events"
        };
    }

    private static string GetFestivalsSubCategoryDescription(FestivalsSubCategory category)
    {
        return category switch
        {
            FestivalsSubCategory.MusicFestivals => "Music festival events",
            FestivalsSubCategory.FilmFestivals => "Film festival events",
            FestivalsSubCategory.ArtFestivals => "Art festival events",
            FestivalsSubCategory.FoodAndWineFestivals => "Food and wine festivals",
            FestivalsSubCategory.CulturalFestivals => "Cultural festival events",
            FestivalsSubCategory.FolkloreFestivals => "Folklore festival events",
            FestivalsSubCategory.StreetFestivals => "Street festival events",
            FestivalsSubCategory.SummerFestivals => "Summer festival events",
            FestivalsSubCategory.LightFestivals => "Light festival events",
            FestivalsSubCategory.CraftBeerFestivals => "Craft beer festivals",
            FestivalsSubCategory.EcoFestivals => "Eco festival events",
            FestivalsSubCategory.DanceFestivals => "Dance festival events",
            FestivalsSubCategory.TechFestivals => "Tech festival events",
            FestivalsSubCategory.Other => "Other festival events",
            _ => $"{category} festival events"
        };
    }

    private static string GetExhibitionsSubCategoryDescription(ExhibitionsSubCategory category)
    {
        return category switch
        {
            ExhibitionsSubCategory.ArtExhibitions => "Art exhibitions",
            ExhibitionsSubCategory.PhotographyExhibitions => "Photography exhibitions",
            ExhibitionsSubCategory.HistoricalExhibitions => "Historical exhibitions",
            ExhibitionsSubCategory.ScienceExhibitions => "Science exhibitions",
            ExhibitionsSubCategory.TechnologyExhibitions => "Technology exhibitions",
            ExhibitionsSubCategory.AutomotiveExhibitions => "Automotive exhibitions",
            ExhibitionsSubCategory.DesignExhibitions => "Design exhibitions",
            ExhibitionsSubCategory.CulturalHeritageExhibitions => "Cultural heritage exhibitions",
            ExhibitionsSubCategory.EducationalExhibitions => "Educational exhibitions",
            ExhibitionsSubCategory.CraftExhibitions => "Craft exhibitions",
            ExhibitionsSubCategory.Other => "Other exhibitions",
            _ => $"{category} exhibitions"
        };
    }

    private static string GetConferencesSubCategoryDescription(ConferencesSubCategory category)
    {
        return category switch
        {
            ConferencesSubCategory.TechConferences => "Technology conferences",
            ConferencesSubCategory.BusinessConferences => "Business conferences",
            ConferencesSubCategory.StartupConferences => "Startup conferences",
            ConferencesSubCategory.AcademicConferences => "Academic conferences",
            ConferencesSubCategory.MarketingConferences => "Marketing conferences",
            ConferencesSubCategory.ScienceConferences => "Science conferences",
            ConferencesSubCategory.HealthAndMedicineConferences => "Health and medicine conferences",
            ConferencesSubCategory.AIAndInnovationConferences => "AI and innovation conferences",
            ConferencesSubCategory.ITSecurityConferences => "IT security conferences",
            ConferencesSubCategory.EnvironmentalConferences => "Environmental conferences",
            ConferencesSubCategory.HRConferences => "HR conferences",
            ConferencesSubCategory.Other => "Other conferences",
            _ => $"{category} conferences"
        };
    }

    private static string GetWorkshopsSubCategoryDescription(WorkshopsSubCategory category)
    {
        return category switch
        {
            WorkshopsSubCategory.ArtWorkshops => "Art workshops",
            WorkshopsSubCategory.MusicWorkshops => "Music workshops",
            WorkshopsSubCategory.DanceWorkshops => "Dance workshops",
            WorkshopsSubCategory.PhotographyWorkshops => "Photography workshops",
            WorkshopsSubCategory.CookingWorkshops => "Cooking workshops",
            WorkshopsSubCategory.CraftWorkshops => "Craft workshops",
            WorkshopsSubCategory.StartupAndEntrepreneurshipWorkshops => "Startup and entrepreneurship workshops",
            WorkshopsSubCategory.PersonalDevelopmentWorkshops => "Personal development workshops",
            WorkshopsSubCategory.CodingWorkshops => "Coding workshops",
            WorkshopsSubCategory.LanguageWorkshops => "Language workshops",
            WorkshopsSubCategory.TheatreWorkshops => "Theatre workshops",
            WorkshopsSubCategory.YogaAndWellnessWorkshops => "Yoga and wellness workshops",
            WorkshopsSubCategory.MarketingWorkshops => "Marketing workshops",
            WorkshopsSubCategory.Other => "Other workshops",
            _ => $"{category} workshops"
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
using Events.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Events.Crawler.Services;

public static class SubCategoryMapper
{
    public static int? MapSubCategoryToEnumValue(EventCategory category, string? subCategoryName, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(subCategoryName))
        {
            logger.LogWarning("SubCategory name is null or empty for category {Category}, defaulting to Other", category);
            return GetOtherEnumValue(category);
        }

        var normalized = NormalizeSubCategoryName(subCategoryName);

        var enumValue = category switch
        {
            EventCategory.Music => MapMusicSubCategory(normalized),
            EventCategory.Art => MapArtSubCategory(normalized),
            EventCategory.Business => MapBusinessSubCategory(normalized),
            EventCategory.Sports => MapSportsSubCategory(normalized),
            EventCategory.Theatre => MapTheatreSubCategory(normalized),
            EventCategory.Cinema => MapCinemaSubCategory(normalized),
            EventCategory.Festivals => MapFestivalsSubCategory(normalized),
            EventCategory.Exhibitions => MapExhibitionsSubCategory(normalized),
            EventCategory.Conferences => MapConferencesSubCategory(normalized),
            EventCategory.Workshops => MapWorkshopsSubCategory(normalized),
            _ => null
        };

        if (enumValue == null)
        {
            logger.LogWarning("Could not map subcategory '{SubCategory}' for category {Category}, using Other",
                subCategoryName, category);
            return GetOtherEnumValue(category);
        }

        // Validate that the enum value belongs to the correct category
        if (!ValidateSubCategoryBelongsToCategory(category, enumValue.Value))
        {
            logger.LogError("VALIDATION ERROR: SubCategory '{SubCategory}' (enum value {EnumValue}) does NOT belong to category {Category}! Using Other instead.",
                subCategoryName, enumValue.Value, category);
            return GetOtherEnumValue(category);
        }

        return enumValue;
    }

    private static bool ValidateSubCategoryBelongsToCategory(EventCategory category, int enumValue)
    {
        return category switch
        {
            EventCategory.Music => Enum.IsDefined(typeof(MusicSubCategory), enumValue),
            EventCategory.Art => Enum.IsDefined(typeof(ArtSubCategory), enumValue),
            EventCategory.Business => Enum.IsDefined(typeof(BusinessSubCategory), enumValue),
            EventCategory.Sports => Enum.IsDefined(typeof(SportsSubCategory), enumValue),
            EventCategory.Theatre => Enum.IsDefined(typeof(TheatreSubCategory), enumValue),
            EventCategory.Cinema => Enum.IsDefined(typeof(CinemaSubCategory), enumValue),
            EventCategory.Festivals => Enum.IsDefined(typeof(FestivalsSubCategory), enumValue),
            EventCategory.Exhibitions => Enum.IsDefined(typeof(ExhibitionsSubCategory), enumValue),
            EventCategory.Conferences => Enum.IsDefined(typeof(ConferencesSubCategory), enumValue),
            EventCategory.Workshops => Enum.IsDefined(typeof(WorkshopsSubCategory), enumValue),
            _ => false
        };
    }

    private static string NormalizeSubCategoryName(string name)
    {
        return name.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "")
            .ToLowerInvariant();
    }

    private static int? MapMusicSubCategory(string normalized)
    {
        return normalized switch
        {
            "pop" => (int)MusicSubCategory.Pop,
            "rock" => (int)MusicSubCategory.Rock,
            "hiphop" => (int)MusicSubCategory.HipHop,
            "rap" => (int)MusicSubCategory.Rap,
            "jazz" => (int)MusicSubCategory.Jazz,
            "blues" => (int)MusicSubCategory.Blues,
            "classical" => (int)MusicSubCategory.Classical,
            "folk" => (int)MusicSubCategory.Folk,
            "traditionalbulgarian" or "bulgarianfolk" or "nationalfolklore" => (int)MusicSubCategory.TraditionalBulgarian,
            "edm" or "electronicdancemusic" => (int)MusicSubCategory.EDM,
            "techno" => (int)MusicSubCategory.Techno,
            "house" => (int)MusicSubCategory.House,
            "drumbass" or "drumandbass" => (int)MusicSubCategory.DrumBass,
            "trance" => (int)MusicSubCategory.Trance,
            "reggae" => (int)MusicSubCategory.Reggae,
            "rb" or "rnb" or "r&b" => (int)MusicSubCategory.RB,
            "metal" => (int)MusicSubCategory.Metal,
            "indie" => (int)MusicSubCategory.Indie,
            "acoustic" => (int)MusicSubCategory.Acoustic,
            "alternative" => (int)MusicSubCategory.Alternative,
            "punk" => (int)MusicSubCategory.Punk,
            "soul" => (int)MusicSubCategory.Soul,
            "chillout" => (int)MusicSubCategory.Chillout,
            "experimental" => (int)MusicSubCategory.Experimental,
            "choir" => (int)MusicSubCategory.Choir,
            "worldmusic" => (int)MusicSubCategory.WorldMusic,
            "other" => (int)MusicSubCategory.Other,
            _ => null
        };
    }

    private static int? MapArtSubCategory(string normalized)
    {
        return normalized switch
        {
            "painting" or "???????" => (int)ArtSubCategory.Painting,
            "sculpture" or "?????????" => (int)ArtSubCategory.Sculpture,
            "photography" or "??????????" => (int)ArtSubCategory.Photography,
            "digitalart" or "?????????????????" => (int)ArtSubCategory.DigitalArt,
            "streetart" or "??????????????" => (int)ArtSubCategory.StreetArt,
            "graffiti" or "???????" => (int)ArtSubCategory.Graffiti,
            "illustration" or "??????????" => (int)ArtSubCategory.Illustration,
            "performanceart" or "??????????" => (int)ArtSubCategory.PerformanceArt,
            "installationart" or "??????????" => (int)ArtSubCategory.InstallationArt,
            "contemporaryart" or "??????????????????" => (int)ArtSubCategory.ContemporaryArt,
            "visualarts" or "????????????????" => (int)ArtSubCategory.VisualArts,
            "mixedmedia" or "??????????????" => (int)ArtSubCategory.MixedMedia,
            "conceptualart" or "????????????" => (int)ArtSubCategory.ConceptualArt,
            "other" or "?????" => (int)ArtSubCategory.Other,
            _ => null
        };
    }

    private static int? MapBusinessSubCategory(string normalized)
    {
        return normalized switch
        {
            "networkingevents" or "networking" or "??????????" => (int)BusinessSubCategory.NetworkingEvents,
            "startups" or "????????" => (int)BusinessSubCategory.Startups,
            "entrepreneurship" or "????????????????" => (int)BusinessSubCategory.Entrepreneurship,
            "marketing" or "?????????" => (int)BusinessSubCategory.Marketing,
            "sales" or "????????" => (int)BusinessSubCategory.Sales,
            "leadership" or "?????????" => (int)BusinessSubCategory.Leadership,
            "finance" or "???????" => (int)BusinessSubCategory.Finance,
            "realestate" or "??????????????" => (int)BusinessSubCategory.RealEstate,
            "investment" or "??????????" => (int)BusinessSubCategory.Investment,
            "ecommerce" or "??????????????????" => (int)BusinessSubCategory.ECommerce,
            "innovation" or "????????" => (int)BusinessSubCategory.Innovation,
            "technology" or "??????????" => (int)BusinessSubCategory.Technology,
            "hrmanagement" or "hr" or "??????????????" => (int)BusinessSubCategory.HRManagement,
            "businessstrategy" or "?????????" => (int)BusinessSubCategory.BusinessStrategy,
            "productdevelopment" or "??????????????????" => (int)BusinessSubCategory.ProductDevelopment,
            "other" or "?????" => (int)BusinessSubCategory.Other,
            _ => null
        };
    }

    private static int? MapSportsSubCategory(string normalized)
    {
        return normalized switch
        {
            "football" => (int)SportsSubCategory.Football,
            "basketball" => (int)SportsSubCategory.Basketball,
            "volleyball" => (int)SportsSubCategory.Volleyball,
            "tennis" => (int)SportsSubCategory.Tennis,
            "athletics" => (int)SportsSubCategory.Athletics,
            "swimming" => (int)SportsSubCategory.Swimming,
            "running" => (int)SportsSubCategory.Running,
            "cycling" => (int)SportsSubCategory.Cycling,
            "boxing" => (int)SportsSubCategory.Boxing,
            "mma" => (int)SportsSubCategory.MMA,
            "wrestling" => (int)SportsSubCategory.Wrestling,
            "weightlifting" => (int)SportsSubCategory.Weightlifting,
            "crossfit" => (int)SportsSubCategory.CrossFit,
            "yoga" => (int)SportsSubCategory.Yoga,
            "fitness" => (int)SportsSubCategory.Fitness,
            "hiking" => (int)SportsSubCategory.Hiking,
            "climbing" => (int)SportsSubCategory.Climbing,
            "skiing" => (int)SportsSubCategory.Skiing,
            "snowboarding" => (int)SportsSubCategory.Snowboarding,
            "motocross" => (int)SportsSubCategory.Motocross,
            "esports" or "egames" or "gaming" => (int)SportsSubCategory.eSports,
            "tabletennis" => (int)SportsSubCategory.TableTennis,
            "badminton" => (int)SportsSubCategory.Badminton,
            "golf" => (int)SportsSubCategory.Golf,
            "dancesport" => (int)SportsSubCategory.DanceSport,
            "other" => (int)SportsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapTheatreSubCategory(string normalized)
    {
        return normalized switch
        {
            "drama" => (int)TheatreSubCategory.Drama,
            "comedy" => (int)TheatreSubCategory.Comedy,
            "musicaltheatre" => (int)TheatreSubCategory.MusicalTheatre,
            "tragedy" => (int)TheatreSubCategory.Tragedy,
            "experimentaltheatre" or "experimental" => (int)TheatreSubCategory.ExperimentalTheatre,
            "puppettheatre" or "puppet" => (int)TheatreSubCategory.PuppetTheatre,
            "improvisation" or "improv" => (int)TheatreSubCategory.Improvisation,
            "streettheatre" or "street" => (int)TheatreSubCategory.StreetTheatre,
            "monodrama" => (int)TheatreSubCategory.Monodrama,
            "childrenstheatre" or "children" or "kids" => (int)TheatreSubCategory.ChildrensTheatre,
            "standupcomedy" or "standup" => (int)TheatreSubCategory.StandupComedy,
            "other" => (int)TheatreSubCategory.Other,
            _ => null
        };
    }

    private static int? MapCinemaSubCategory(string normalized)
    {
        return normalized switch
        {
            "featurefilms" or "????????????" => (int)CinemaSubCategory.FeatureFilms,
            "shortfilms" or "????????????" => (int)CinemaSubCategory.ShortFilms,
            "documentaries" or "????????????" => (int)CinemaSubCategory.Documentaries,
            "animation" or "????????" => (int)CinemaSubCategory.Animation,
            "independentcinema" or "??????????" => (int)CinemaSubCategory.IndependentCinema,
            "bulgariancinema" or "?????????????" => (int)CinemaSubCategory.BulgarianCinema,
            "internationalcinema" or "????????????" => (int)CinemaSubCategory.InternationalCinema,
            "filmpremieres" or "????????" => (int)CinemaSubCategory.FilmPremieres,
            "studentfilms" or "??????????" => (int)CinemaSubCategory.StudentFilms,
            "filmfestivals" or "????????????????" => (int)CinemaSubCategory.FilmFestivals,
            "other" or "?????" => (int)CinemaSubCategory.Other,
            _ => null
        };
    }

    private static int? MapFestivalsSubCategory(string normalized)
    {
        return normalized switch
        {
            "musicfestivals" or "??????????????????" => (int)FestivalsSubCategory.MusicFestivals,
            "filmfestivals" or "????????????????" => (int)FestivalsSubCategory.FilmFestivals,
            "artfestivals" or "????????????" => (int)FestivalsSubCategory.ArtFestivals,
            "foodandwinefestivals" or "?????????" => (int)FestivalsSubCategory.FoodAndWineFestivals,
            "culturalfestivals" or "????????" => (int)FestivalsSubCategory.CulturalFestivals,
            "folklorefestivals" or "?????????" => (int)FestivalsSubCategory.FolkloreFestivals,
            "streetfestivals" or "??????" => (int)FestivalsSubCategory.StreetFestivals,
            "summerfestivals" or "?????" => (int)FestivalsSubCategory.SummerFestivals,
            "lightfestivals" or "?????????" => (int)FestivalsSubCategory.LightFestivals,
            "craftbeerfestivals" or "????" => (int)FestivalsSubCategory.CraftBeerFestivals,
            "ecofestivals" or "???" => (int)FestivalsSubCategory.EcoFestivals,
            "dancefestivals" or "???????" => (int)FestivalsSubCategory.DanceFestivals,
            "techfestivals" or "????????????" => (int)FestivalsSubCategory.TechFestivals,
            "other" or "?????" => (int)FestivalsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapExhibitionsSubCategory(string normalized)
    {
        return normalized switch
        {
            "artexhibitions" or "??????????" => (int)ExhibitionsSubCategory.ArtExhibitions,
            "photographyexhibitions" or "???????????" => (int)ExhibitionsSubCategory.PhotographyExhibitions,
            "historicalexhibitions" or "???????????" => (int)ExhibitionsSubCategory.HistoricalExhibitions,
            "scienceexhibitions" or "??????" => (int)ExhibitionsSubCategory.ScienceExhibitions,
            "technologyexhibitions" or "????????????" => (int)ExhibitionsSubCategory.TechnologyExhibitions,
            "automotiveexhibitions" or "???????????" => (int)ExhibitionsSubCategory.AutomotiveExhibitions,
            "designexhibitions" or "??????" => (int)ExhibitionsSubCategory.DesignExhibitions,
            "culturalheritageexhibitions" or "??????????????????" => (int)ExhibitionsSubCategory.CulturalHeritageExhibitions,
            "educationalexhibitions" or "?????????????" => (int)ExhibitionsSubCategory.EducationalExhibitions,
            "craftexhibitions" or "???????" => (int)ExhibitionsSubCategory.CraftExhibitions,
            "other" or "?????" => (int)ExhibitionsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapConferencesSubCategory(string normalized)
    {
        return normalized switch
        {
            "techconferences" or "????????????" => (int)ConferencesSubCategory.TechConferences,
            "businessconferences" or "??????" => (int)ConferencesSubCategory.BusinessConferences,
            "startupconferences" or "???????" => (int)ConferencesSubCategory.StartupConferences,
            "academicconferences" or "??????????" => (int)ConferencesSubCategory.AcademicConferences,
            "marketingconferences" or "?????????" => (int)ConferencesSubCategory.MarketingConferences,
            "scienceconferences" or "??????" => (int)ConferencesSubCategory.ScienceConferences,
            "healthandmedicineconferences" or "??????" or "????????" => (int)ConferencesSubCategory.HealthAndMedicineConferences,
            "aiandinnovationconferences" or "ai" or "?????????????????" => (int)ConferencesSubCategory.AIAndInnovationConferences,
            "itsecurityconferences" or "?????????" => (int)ConferencesSubCategory.ITSecurityConferences,
            "environmentalconferences" or "???????????" => (int)ConferencesSubCategory.EnvironmentalConferences,
            "hrconferences" or "hr" or "??????????????" => (int)ConferencesSubCategory.HRConferences,
            "other" or "?????" => (int)ConferencesSubCategory.Other,
            _ => null
        };
    }

    private static int? MapWorkshopsSubCategory(string normalized)
    {
        return normalized switch
        {
            "artworkshops" or "??????????????" => (int)WorkshopsSubCategory.ArtWorkshops,
            "musicworkshops" or "?????????" => (int)WorkshopsSubCategory.MusicWorkshops,
            "danceworkshops" or "???????" => (int)WorkshopsSubCategory.DanceWorkshops,
            "photographyworkshops" or "??????????" => (int)WorkshopsSubCategory.PhotographyWorkshops,
            "cookingworkshops" or "?????????" => (int)WorkshopsSubCategory.CookingWorkshops,
            "craftworkshops" or "???????" => (int)WorkshopsSubCategory.CraftWorkshops,
            "startupandentrepreneurshipworkshops" or "???????" or "????????????????" => (int)WorkshopsSubCategory.StartupAndEntrepreneurshipWorkshops,
            "personaldevelopmentworkshops" or "?????????????" => (int)WorkshopsSubCategory.PersonalDevelopmentWorkshops,
            "codingworkshops" or "????????????" => (int)WorkshopsSubCategory.CodingWorkshops,
            "languageworkshops" or "???????" => (int)WorkshopsSubCategory.LanguageWorkshops,
            "theatreworkshops" or "?????????" => (int)WorkshopsSubCategory.TheatreWorkshops,
            "yogaandwellnessworkshops" or "????" or "??????" => (int)WorkshopsSubCategory.YogaAndWellnessWorkshops,
            "marketingworkshops" or "?????????" => (int)WorkshopsSubCategory.MarketingWorkshops,
            "other" or "?????" => (int)WorkshopsSubCategory.Other,
            _ => null
        };
    }

    private static int GetOtherEnumValue(EventCategory category)
    {
        return 99; // All "Other" enums have value 99
    }
}

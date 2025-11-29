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

        return enumValue;
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
            "rock" or "???" => (int)MusicSubCategory.Rock,
            "jazz" or "????" => (int)MusicSubCategory.Jazz,
            "metal" or "?????" or "heavymetal" or "thrashmetal" => (int)MusicSubCategory.Metal,
            "pop" or "???" => (int)MusicSubCategory.Pop,
            "funk" or "????" => (int)MusicSubCategory.Funk,
            "punk" or "????" => (int)MusicSubCategory.Punk,
            "opera" or "?????" => (int)MusicSubCategory.Opera,
            "classical" or "??????????" or "???????" => (int)MusicSubCategory.Classical,
            "electronic" or "??????????" or "techno" or "house" or "???" or "edm" => (int)MusicSubCategory.Electronic,
            "folk" or "????" or "???????" => (int)MusicSubCategory.Folk,
            "blues" or "????" => (int)MusicSubCategory.Blues,
            "country" or "??????" => (int)MusicSubCategory.Country,
            "reggae" or "?????" => (int)MusicSubCategory.Reggae,
            "hiphop" or "??????" or "rap" or "???" => (int)MusicSubCategory.HipHop,
            "alternative" or "????????????" => (int)MusicSubCategory.Alternative,
            "other" or "?????" => (int)MusicSubCategory.Other,
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
            "football" or "??????" => (int)SportsSubCategory.Football,
            "basketball" or "?????????" => (int)SportsSubCategory.Basketball,
            "tennis" or "?????" => (int)SportsSubCategory.Tennis,
            "volleyball" or "????????" => (int)SportsSubCategory.Volleyball,
            "swimming" or "???????" => (int)SportsSubCategory.Swimming,
            "athletics" or "????????" => (int)SportsSubCategory.Athletics,
            "boxing" or "????" => (int)SportsSubCategory.Boxing,
            "wrestling" or "?????" => (int)SportsSubCategory.Wrestling,
            "gymnastics" or "??????????" => (int)SportsSubCategory.Gymnastics,
            "cycling" or "??????????" => (int)SportsSubCategory.Cycling,
            "other" or "?????" => (int)SportsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapTheatreSubCategory(string normalized)
    {
        return normalized switch
        {
            "drama" or "?????" => (int)TheatreSubCategory.Drama,
            "comedy" or "???????" => (int)TheatreSubCategory.Comedy,
            "musicaltheatre" or "???????" => (int)TheatreSubCategory.MusicalTheatre,
            "tragedy" or "????????" => (int)TheatreSubCategory.Tragedy,
            "experimentaltheatre" or "???????????????" => (int)TheatreSubCategory.ExperimentalTheatre,
            "puppettheatre" or "????????????" => (int)TheatreSubCategory.PuppetTheatre,
            "improvisation" or "????????????" => (int)TheatreSubCategory.Improvisation,
            "streettheatre" or "????????????" => (int)TheatreSubCategory.StreetTheatre,
            "monodrama" or "?????????" => (int)TheatreSubCategory.Monodrama,
            "childrenstheatre" or "????????????" => (int)TheatreSubCategory.ChildrensTheatre,
            "other" or "?????" => (int)TheatreSubCategory.Other,
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

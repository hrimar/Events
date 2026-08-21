using Events.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Events.Services.Helpers;

public static class SubCategoryMapper
{
    public static int? MapSubCategoryToEnumValue(EventCategory category, string? subCategoryName, ILogger logger, bool allowOtherFallback = true)
    {
        if (string.IsNullOrWhiteSpace(subCategoryName))
        {
            if (!allowOtherFallback)
            {
                return null;
            }

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
            EventCategory.Entertainment => MapEntertainmentSubCategory(normalized),
            EventCategory.FoodDrink => MapFoodDrinkSubCategory(normalized),
            EventCategory.Markets => MapMarketsSubCategory(normalized),
            _ => null
        };

        if (enumValue == null)
        {
            if (!allowOtherFallback)
            {
                return null;
            }

            logger.LogWarning("Could not map subcategory '{SubCategory}' for category {Category}, using Other",
                subCategoryName, category);
            return GetOtherEnumValue(category);
        }

        // Validate that the enum value belongs to the correct category
        if (!ValidateSubCategoryBelongsToCategory(category, enumValue.Value))
        {
            logger.LogError("VALIDATION ERROR: SubCategory '{SubCategory}' (enum value {EnumValue}) does NOT belong to category {Category}! Using Other instead.",
                subCategoryName, enumValue.Value, category);

            if (!allowOtherFallback)
            {
                return null;
            }

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
            EventCategory.Entertainment => Enum.IsDefined(typeof(EntertainmentSubCategory), enumValue),
            EventCategory.FoodDrink => Enum.IsDefined(typeof(FoodDrinkSubCategory), enumValue),
            EventCategory.Markets => Enum.IsDefined(typeof(MarketsSubCategory), enumValue),
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
            "painting" or "рисуване" or "живопис" => (int)ArtSubCategory.Painting,
            "sculpture" or "скулптура" => (int)ArtSubCategory.Sculpture,
            "photography" or "фотография" => (int)ArtSubCategory.Photography,
            "digitalart" or "дигиталноизкуство" => (int)ArtSubCategory.DigitalArt,
            "streetart" or "уличноизкуство" => (int)ArtSubCategory.StreetArt,
            "graffiti" or "графити" => (int)ArtSubCategory.Graffiti,
            "illustration" or "илюстрация" => (int)ArtSubCategory.Illustration,
            "performanceart" or "пърформанс" or "перформанс" => (int)ArtSubCategory.PerformanceArt,
            "installationart" or "инсталация" => (int)ArtSubCategory.InstallationArt,
            "contemporaryart" or "съвременноизкуство" => (int)ArtSubCategory.ContemporaryArt,
            "visualarts" or "визуалниизкуства" => (int)ArtSubCategory.VisualArts,
            "mixedmedia" or "смесенатехника" => (int)ArtSubCategory.MixedMedia,
            "conceptualart" or "концептуалноизкуство" => (int)ArtSubCategory.ConceptualArt,
            "other" or "друго" => (int)ArtSubCategory.Other,
            _ => null
        };
    }

    private static int? MapBusinessSubCategory(string normalized)
    {
        return normalized switch
        {
            "networkingevents" or "networking" or "нетуъркинг" => (int)BusinessSubCategory.NetworkingEvents,
            "startups" or "стартъпи" => (int)BusinessSubCategory.Startups,
            "entrepreneurship" or "предприемачество" => (int)BusinessSubCategory.Entrepreneurship,
            "marketing" or "маркетинг" => (int)BusinessSubCategory.Marketing,
            "sales" or "продажби" => (int)BusinessSubCategory.Sales,
            "leadership" or "лидерство" => (int)BusinessSubCategory.Leadership,
            "finance" or "финанси" => (int)BusinessSubCategory.Finance,
            "realestate" or "недвижимиимоти" => (int)BusinessSubCategory.RealEstate,
            "investment" or "инвестиции" => (int)BusinessSubCategory.Investment,
            "ecommerce" or "електроннатърговия" => (int)BusinessSubCategory.ECommerce,
            "innovation" or "иновации" => (int)BusinessSubCategory.Innovation,
            "technology" or "технологии" => (int)BusinessSubCategory.Technology,
            "hrmanagement" or "hr" or "човешкиресурси" => (int)BusinessSubCategory.HRManagement,
            "businessstrategy" or "бизнесстратегия" => (int)BusinessSubCategory.BusinessStrategy,
            "productdevelopment" or "разработканапродукти" => (int)BusinessSubCategory.ProductDevelopment,
            "other" or "друго" => (int)BusinessSubCategory.Other,
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
            "featurefilms" or "игралифилми" => (int)CinemaSubCategory.FeatureFilms,
            "shortfilms" or "късометражнифилми" => (int)CinemaSubCategory.ShortFilms,
            "documentaries" or "документалнифилми" => (int)CinemaSubCategory.Documentaries,
            "animation" or "анимация" => (int)CinemaSubCategory.Animation,
            "independentcinema" or "независимокино" => (int)CinemaSubCategory.IndependentCinema,
            "bulgariancinema" or "българскокино" => (int)CinemaSubCategory.BulgarianCinema,
            "internationalcinema" or "чуждестраннокино" => (int)CinemaSubCategory.InternationalCinema,
            "filmpremieres" or "премиери" => (int)CinemaSubCategory.FilmPremieres,
            "studentfilms" or "студентскифилми" => (int)CinemaSubCategory.StudentFilms,
            "filmfestivals" or "филмовифестивали" => (int)CinemaSubCategory.FilmFestivals,
            "other" or "друго" => (int)CinemaSubCategory.Other,
            _ => null
        };
    }

    private static int? MapFestivalsSubCategory(string normalized)
    {
        return normalized switch
        {
            "musicfestivals" or "музикалнифестивали" => (int)FestivalsSubCategory.MusicFestivals,
            "filmfestivals" or "филмовифестивали" => (int)FestivalsSubCategory.FilmFestivals,
            "artfestivals" or "фестивалинаизкуството" => (int)FestivalsSubCategory.ArtFestivals,
            "foodandwinefestivals" or "гастрономическифестивали" => (int)FestivalsSubCategory.FoodAndWineFestivals,
            "culturalfestivals" or "културнифестивали" => (int)FestivalsSubCategory.CulturalFestivals,
            "folklorefestivals" or "фолклорнифестивали" => (int)FestivalsSubCategory.FolkloreFestivals,
            "streetfestivals" or "уличнифестивали" => (int)FestivalsSubCategory.StreetFestivals,
            "summerfestivals" or "летнифестивали" => (int)FestivalsSubCategory.SummerFestivals,
            "lightfestivals" or "светлиннифестивали" => (int)FestivalsSubCategory.LightFestivals,
            "craftbeerfestivals" or "бирафестивали" => (int)FestivalsSubCategory.CraftBeerFestivals,
            "ecofestivals" or "екофестивали" => (int)FestivalsSubCategory.EcoFestivals,
            "dancefestivals" or "танцовифестивали" => (int)FestivalsSubCategory.DanceFestivals,
            "techfestivals" or "технологичнифестивали" => (int)FestivalsSubCategory.TechFestivals,
            "other" or "друго" => (int)FestivalsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapExhibitionsSubCategory(string normalized)
    {
        return normalized switch
        {
            "artexhibitions" or "художественизложби" => (int)ExhibitionsSubCategory.ArtExhibitions,
            "photographyexhibitions" or "фотоизложби" => (int)ExhibitionsSubCategory.PhotographyExhibitions,
            "historicalexhibitions" or "историческиизложби" => (int)ExhibitionsSubCategory.HistoricalExhibitions,
            "scienceexhibitions" or "научниизложби" => (int)ExhibitionsSubCategory.ScienceExhibitions,
            "technologyexhibitions" or "технологичниизложби" => (int)ExhibitionsSubCategory.TechnologyExhibitions,
            "automotiveexhibitions" or "автомобилниизложби" => (int)ExhibitionsSubCategory.AutomotiveExhibitions,
            "designexhibitions" or "дизайнерскиизложби" => (int)ExhibitionsSubCategory.DesignExhibitions,
            "culturalheritageexhibitions" or "изложбизакултурнонаследство" => (int)ExhibitionsSubCategory.CulturalHeritageExhibitions,
            "educationalexhibitions" or "образователниизложби" => (int)ExhibitionsSubCategory.EducationalExhibitions,
            "craftexhibitions" or "занаятчийскиизложби" => (int)ExhibitionsSubCategory.CraftExhibitions,
            "other" or "друго" => (int)ExhibitionsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapConferencesSubCategory(string normalized)
    {
        return normalized switch
        {
            "techconferences" or "технологичниконференции" => (int)ConferencesSubCategory.TechConferences,
            "businessconferences" or "бизнесконференции" => (int)ConferencesSubCategory.BusinessConferences,
            "startupconferences" or "стартъпконференции" => (int)ConferencesSubCategory.StartupConferences,
            "academicconferences" or "академичниконференции" => (int)ConferencesSubCategory.AcademicConferences,
            "marketingconferences" or "маркетинговиконференции" => (int)ConferencesSubCategory.MarketingConferences,
            "scienceconferences" or "научниконференции" => (int)ConferencesSubCategory.ScienceConferences,
            "healthandmedicineconferences" or "здраве" or "медицина" => (int)ConferencesSubCategory.HealthAndMedicineConferences,
            "aiandinnovationconferences" or "ai" or "изкуственинтелектииновации" => (int)ConferencesSubCategory.AIAndInnovationConferences,
            "itsecurityconferences" or "киберсигурност" => (int)ConferencesSubCategory.ITSecurityConferences,
            "environmentalconferences" or "екологичниконференции" => (int)ConferencesSubCategory.EnvironmentalConferences,
            "hrconferences" or "hr" or "човешкиресурсиконференции" => (int)ConferencesSubCategory.HRConferences,
            "other" or "друго" => (int)ConferencesSubCategory.Other,
            _ => null
        };
    }

    private static int? MapWorkshopsSubCategory(string normalized)
    {
        return normalized switch
        {
            "artworkshops" or "художественработилници" => (int)WorkshopsSubCategory.ArtWorkshops,
            "musicworkshops" or "музикалниработилници" => (int)WorkshopsSubCategory.MusicWorkshops,
            "danceworkshops" or "танцовиработилници" => (int)WorkshopsSubCategory.DanceWorkshops,
            "photographyworkshops" or "фотоработилници" => (int)WorkshopsSubCategory.PhotographyWorkshops,
            "cookingworkshops" or "кулинарниработилници" => (int)WorkshopsSubCategory.CookingWorkshops,
            "craftworkshops" or "занаятчийскиработилници" => (int)WorkshopsSubCategory.CraftWorkshops,
            "startupandentrepreneurshipworkshops" or "стартъп" or "предприемачество" => (int)WorkshopsSubCategory.StartupAndEntrepreneurshipWorkshops,
            "personaldevelopmentworkshops" or "личностноразвитие" => (int)WorkshopsSubCategory.PersonalDevelopmentWorkshops,
            "codingworkshops" or "програмиране" => (int)WorkshopsSubCategory.CodingWorkshops,
            "languageworkshops" or "езиковиработилници" => (int)WorkshopsSubCategory.LanguageWorkshops,
            "theatreworkshops" or "театралниработилници" => (int)WorkshopsSubCategory.TheatreWorkshops,
            "yogaandwellnessworkshops" or "йога" or "уелнес" => (int)WorkshopsSubCategory.YogaAndWellnessWorkshops,
            "marketingworkshops" or "маркетинговиработилници" => (int)WorkshopsSubCategory.MarketingWorkshops,
            "other" or "друго" => (int)WorkshopsSubCategory.Other,
            _ => null
        };
    }

    private static int? MapEntertainmentSubCategory(string normalized)
    {
        return normalized switch
        {
            "party" or "парти" => (int)EntertainmentSubCategory.Party,
            "themedparty" or "тематичнопарти" => (int)EntertainmentSubCategory.ThemedParty,
            "clubnight" or "клубнавечер" => (int)EntertainmentSubCategory.ClubNight,
            "quiz" or "куизитривия" => (int)EntertainmentSubCategory.Quiz,
            "karaoke" or "караоке" => (int)EntertainmentSubCategory.Karaoke,
            "boardgames" or "настолниигри" => (int)EntertainmentSubCategory.BoardGames,
            "escaperoom" or "ескейпрум" => (int)EntertainmentSubCategory.EscapeRoom,
            "socialdance" or "социалнитанци" => (int)EntertainmentSubCategory.SocialDance,
            "other" or "друго" => (int)EntertainmentSubCategory.Other,
            _ => null
        };
    }

    private static int? MapFoodDrinkSubCategory(string normalized)
    {
        return normalized switch
        {
            "winetasting" or "дегустациянавино" => (int)FoodDrinkSubCategory.WineTasting,
            "beertasting" or "дегустациянабира" => (int)FoodDrinkSubCategory.BeerTasting,
            "spiritstasting" or "дегустациянаспиртнинапитки" => (int)FoodDrinkSubCategory.SpiritsTasting,
            "culinarydinner" or "кулинарнавечеря" => (int)FoodDrinkSubCategory.CulinaryDinner,
            "brunch" or "бранч" => (int)FoodDrinkSubCategory.Brunch,
            "streetfood" or "стрийтфууд" => (int)FoodDrinkSubCategory.StreetFood,
            "coffeetea" or "кафеичай" => (int)FoodDrinkSubCategory.CoffeeTea,
            "other" or "друго" => (int)FoodDrinkSubCategory.Other,
            _ => null
        };
    }

    private static int? MapMarketsSubCategory(string normalized)
    {
        return normalized switch
        {
            "craftmarket" or "хендмейдбазар" => (int)MarketsSubCategory.CraftMarket,
            "farmersmarket" or "фермерскипазар" => (int)MarketsSubCategory.FarmersMarket,
            "fleamarket" or "битакивинтидж" => (int)MarketsSubCategory.FleaMarket,
            "christmasmarket" or "коледенбазар" => (int)MarketsSubCategory.ChristmasMarket,
            "designmarket" or "дизайнмаркет" => (int)MarketsSubCategory.DesignMarket,
            "bookmarket" or "книженбазар" => (int)MarketsSubCategory.BookMarket,
            "other" or "друго" => (int)MarketsSubCategory.Other,
            _ => null
        };
    }

    private static int GetOtherEnumValue(EventCategory category)
    {
        return 99; // All "Other" enums have value 99
    }
}

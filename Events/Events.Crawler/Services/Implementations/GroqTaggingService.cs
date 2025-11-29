using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Events.Crawler.Services.Implementations;

public class GroqTaggingService : IAiTaggingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<GroqTaggingService> _logger;
    private static readonly SemaphoreSlim _rateLimiter = new(3, 3); // Max 3 concurrent requests
    private static DateTime _lastRequest = DateTime.MinValue;
    private static readonly object _lockObject = new();

    public GroqTaggingService(HttpClient http, IConfiguration config, ILogger<GroqTaggingService> logger)
    {
        _http = http;
        _apiKey = config["Groq:ApiKey"] ?? throw new InvalidOperationException("Groq API key not configured");
        _logger = logger;

        _http.BaseAddress = new Uri("https://api.groq.com/");
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<EventCategory?> ClassifyEventAsync(string eventName, string description)
    {
        await _rateLimiter.WaitAsync();
        try
        {
            await EnsureRateLimit();

            // Enhanced prompt with better context and examples
            var prompt = $@"Analyze this Bulgarian event and classify it with subcategory. Return format: ""CATEGORY_NUMBER|SUBCATEGORY_NAME""

Event: {eventName}
Description: {description}

Categories with Subcategories:
1=Music (концерти, музикални изпълнения, bands, DJ sets, festivals with music)
  - Rock, Jazz, Metal, Pop, Classical, Electronic, Folk, Blues, HipHop, Punk, Funk, Opera, Country, Reggae, Alternative
2=Art (изложби, галерии, visual arts, художествени инсталации)
3=Business (networking, бизнес събития, предприемачество, стартъп събития)
4=Sports (спортни събития, мачове, турнири, състезания)
  - Football, Basketball, Tennis, Volleyball, Swimming, Athletics, Boxing, Wrestling, Gymnastics, Cycling
5=Theatre (театрални постановки, драма, комедия, мюзикъли)
6=Cinema (кино прожекции, филми, премиери, документални)
7=Festivals (многодневни фестивали, културни празници, тематични събития)
8=Exhibitions (изложби, музеи, исторически експозиции)
9=Conferences (конференции, семинари, лекции, научни форуми)
10=Workshops (работилници, курсове, обучения, майсторски класове)

Enhanced Examples:
- ""Slayer концерт в Зала 1"" = ""1|Metal"" (Thrash/Heavy Metal concert)
- ""Iron Maiden live in Sofia"" = ""1|Metal"" (Heavy Metal concert)
- ""Джаз вечер с John Coltrane Tribute"" = ""1|Jazz"" (Jazz performance)
- ""Ed Sheeran концерт"" = ""1|Pop"" (Pop music concert)
- ""Софийска филхармония - Бетовен 9-та симфония"" = ""1|Classical"" (Classical music)
- ""Техно парти с Armin van Buuren"" = ""1|Electronic"" (Electronic/Techno music)
- ""Левски - ЦСКА дерби"" = ""4|Football"" (Football match)
- ""ATP турнир по тенис"" = ""4|Tennis"" (Tennis tournament)
- ""Хамлет в Народния театър"" = ""5|"" (Theatre - no specific subcategory)
- ""Изложба на Пикасо в НХГ"" = ""2|"" (Art exhibition - no subcategory)

Rules:
- For Music/Sports events, try to determine specific subcategory
- For other categories, subcategory can be empty
- If uncertain about category, return ""0|""
- Return ONLY the format ""NUMBER|SUBCATEGORY"" (subcategory can be empty)

Return format: ""CATEGORY_NUMBER|SUBCATEGORY_NAME"":";

            var requestBody = new
            {
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                model = "llama-3.1-8b-instant",
                max_tokens = 20, // Increase tokens for subcategory response
                temperature = 0.1
            };

            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var responseText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

                if (!string.IsNullOrEmpty(responseText))
                {
                    var parts = responseText.Split('|');
                    if (parts.Length >= 1 && int.TryParse(parts[0], out var categoryId))
                    {
                        if (categoryId == 0)
                        {
                            _logger.LogWarning("AI could not classify '{EventName}' with confidence", eventName);
                            return null;
                        }

                        if (Enum.IsDefined(typeof(EventCategory), categoryId))
                        {
                            // Store subcategory for later use in GenerateTagsAsync
                            var subcategory = parts.Length > 1 ? parts[1].Trim() : null;

                            _logger.LogInformation("AI classified '{EventName}' as {Category} | {SubCategory}",
                                eventName, (EventCategory)categoryId, subcategory ?? "None");

                            return (EventCategory)categoryId;
                        }
                    }
                }

                _logger.LogWarning("AI returned invalid response: '{Response}' for event '{EventName}'", responseText, eventName);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("AI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            }

            return TryClassifyWithKeywords(eventName, description);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("rate_limit_exceeded"))
        {
            _logger.LogWarning("Rate limit exceeded, falling back to keyword classification");
            return TryClassifyWithKeywords(eventName, description);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
    {
        try
        {
            // Enhanced AI classification that returns both category and subcategory
            var aiResult = await ClassifyEventWithSubcategoryAsync(eventName, description);

            var category = aiResult.Category;
            var suggestedSubCategory = aiResult.SubCategory;

            // Log if AI used "Other" subcategory
            if (suggestedSubCategory == "Other" && category.HasValue)
            {
                _logger.LogInformation("AI selected 'Other' subcategory for '{EventName}' in category {Category} - may need manual review", 
                    eventName, category);
            }

            var tags = ExtractTagsWithKeywords(eventName, description, location, category);
            var musicGenres = category == EventCategory.Music
                ? ExtractMusicGenresWithKeywords(eventName, description)
                : Enumerable.Empty<string>();

            var allTags = tags.Concat(musicGenres).Distinct().Take(6).ToList();

            _logger.LogInformation("Generated comprehensive tags for '{EventName}': Category={Category}, SubCategory={SubCategory}, Tags=[{Tags}]",
                eventName, category?.ToString() ?? "None", suggestedSubCategory ?? "None", string.Join(", ", allTags));

            return new TaggingResult
            {
                SuggestedCategory = category, // AI-suggested category
                SuggestedSubCategory = suggestedSubCategory, // AI-suggested subcategory
                SuggestedTags = allTags, // AI-suggested tags
                Confidence = allTags.ToDictionary(tag => tag, _ => 0.8)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tags with Groq for '{EventName}'", eventName);
            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
    }

    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
    {
        return await Task.FromResult(ExtractMusicGenresWithKeywords(eventName, description));
    }

    private async Task<(EventCategory? Category, string? SubCategory)> ClassifyEventWithSubcategoryAsync(string eventName, string description)
    {
        await _rateLimiter.WaitAsync();
        try
        {
            await EnsureRateLimit();

            var prompt = $@"Classify this Bulgarian event with category and subcategory. Return ONLY format: ""CATEGORY|SUBCATEGORY""

Event: {eventName}
Description: {description}

Categories with ALL Subcategories:
1=Music → Rock, Jazz, Metal, Pop, Funk, Punk, Opera, Classical, Electronic, Folk, Blues, Country, Reggae, HipHop, Alternative, Other
2=Art → Painting, Sculpture, Photography, DigitalArt, StreetArt, Graffiti, Illustration, PerformanceArt, InstallationArt, ContemporaryArt, VisualArts, MixedMedia, ConceptualArt, Other
3=Business → NetworkingEvents, Startups, Entrepreneurship, Marketing, Sales, Leadership, Finance, RealEstate, Investment, ECommerce, Innovation, Technology, HRManagement, BusinessStrategy, ProductDevelopment, Other
4=Sports → Football, Basketball, Tennis, Volleyball, Swimming, Athletics, Boxing, Wrestling, Gymnastics, Cycling, Other
5=Theatre → Drama, Comedy, MusicalTheatre, Tragedy, ExperimentalTheatre, PuppetTheatre, Improvisation, StreetTheatre, Monodrama, ChildrensTheatre, Other
6=Cinema → FeatureFilms, ShortFilms, Documentaries, Animation, IndependentCinema, BulgarianCinema, InternationalCinema, FilmPremieres, StudentFilms, FilmFestivals, Other
7=Festivals → MusicFestivals, FilmFestivals, ArtFestivals, FoodAndWineFestivals, CulturalFestivals, FolkloreFestivals, StreetFestivals, SummerFestivals, LightFestivals, CraftBeerFestivals, EcoFestivals, DanceFestivals, TechFestivals, Other
8=Exhibitions → ArtExhibitions, PhotographyExhibitions, HistoricalExhibitions, ScienceExhibitions, TechnologyExhibitions, AutomotiveExhibitions, DesignExhibitions, CulturalHeritageExhibitions, EducationalExhibitions, CraftExhibitions, Other
9=Conferences → TechConferences, BusinessConferences, StartupConferences, AcademicConferences, MarketingConferences, ScienceConferences, HealthAndMedicineConferences, AIAndInnovationConferences, ITSecurityConferences, EnvironmentalConferences, HRConferences, Other
10=Workshops → ArtWorkshops, MusicWorkshops, DanceWorkshops, PhotographyWorkshops, CookingWorkshops, CraftWorkshops, StartupAndEntrepreneurshipWorkshops, PersonalDevelopmentWorkshops, CodingWorkshops, LanguageWorkshops, TheatreWorkshops, YogaAndWellnessWorkshops, MarketingWorkshops, Other
11=Undefined → (uncategorized events)

Examples:
""Slayer концерт"" → ""1|Metal""
""Metallica live"" → ""1|Metal""
""Iron Maiden в София"" → ""1|Metal""
""Джаз клуб вечер"" → ""1|Jazz""
""Ed Sheeran live"" → ""1|Pop""
""Софийска филхармония - Бетовен"" → ""1|Classical""
""Техно парти с Arмин"" → ""1|Electronic""
""Народна музика концерт"" → ""1|Folk""
""Левски - ЦСКА дерби"" → ""4|Football""
""ATP турнир тенис"" → ""4|Tennis""
""Баскетбол мач"" → ""4|Basketball""
""Хамлет в Народния театър"" → ""5|Drama""
""Комедия в театъра"" → ""5|Comedy""
""Изложба картини Пикасо"" → ""2|Painting""
""Фотоизложба"" → ""2|Photography""
""Стартъп конференция"" → ""9|StartupConferences""
""Кулинарна работилница"" → ""10|CookingWorkshops""
""Програмиране курс"" → ""10|CodingWorkshops""
""Някакво странно събитие"" → ""11|""

ВАЖНИ ПРАВИЛА:
- ВИНАГИ опитай да определиш подкатегория от списъка
- Ако не си сигурен за подкатегория, използвай ""Other""
- Ако не можеш да определиш категория, върни ""11|""
- Върни САМО ""CATEGORY|SUBCATEGORY"" (нищо друго!)

Return format: ""CATEGORY|SUBCATEGORY"":";

            var requestBody = new
            {
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                model = "llama-3.1-8b-instant",
                max_tokens = 20,
                temperature = 0.1
            };

            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var responseText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

                if (!string.IsNullOrEmpty(responseText))
                {
                    _logger.LogInformation("AI raw response for '{EventName}': {Response}", eventName, responseText);

                    var parts = responseText.Split('|');
                    if (parts.Length >= 1 && int.TryParse(parts[0], out var categoryId))
                    {
                        if (categoryId == 11)
                        {
                            _logger.LogWarning("AI classified '{EventName}' as Undefined - requires manual categorization", eventName);
                            return ((EventCategory)11, null);
                        }

                        if (Enum.IsDefined(typeof(EventCategory), categoryId))
                        {
                            var category = (EventCategory)categoryId;
                            var subcategory = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1].Trim() : "Other";

                            if (subcategory == "Other")
                            {
                                _logger.LogInformation("AI used 'Other' subcategory for '{EventName}' in category {Category}", eventName, category);
                            }

                            _logger.LogInformation("AI classified '{EventName}' as {Category} | {SubCategory}", eventName, category, subcategory);

                            return (category, subcategory);
                        }
                    }
                }
            }

            // Fallback to our method for category and subcategory extraction
            _logger.LogWarning("AI failed to classify '{EventName}', using fallback", eventName);
            var fallbackCategory = await ClassifyEventAsync(eventName, description);
            var fallbackSubcategory = fallbackCategory.HasValue
                ? ExtractSubCategory(eventName, description, fallbackCategory.Value) ?? "Other"
                : null;

            return (fallbackCategory, fallbackSubcategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI category+subcategory classification for '{EventName}'", eventName);

            // Fallback to keyword-based classification
            var category = TryClassifyWithKeywords(eventName, description);
            var subcategory = category.HasValue ? ExtractSubCategory(eventName, description, category.Value) ?? "Other" : null;

            return (category, subcategory);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    private EventCategory? TryClassifyWithKeywords(string eventName, string description)
    {
        var text = $"{eventName} {description}".ToLower();

        if (new[] { "концерт", "concert", "live music", "band", "група" }.Any(keyword => text.Contains(keyword)) ||
            new[] { "slayer", "metallica", "iron maiden", "megadeth", "jazz", "джаз" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Music (очевидно)", eventName);
            return EventCategory.Music;
        }

        if (new[] { "мач", "дерби", "турнир", "състезание", "футбол", "баскетбол" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Sports (очевидно)", eventName);
            return EventCategory.Sports;
        }

        if (new[] { "театър", "пиеса", "спектакъл", "драма", "комедия" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Theatre (очевидно)", eventName);
            return EventCategory.Theatre;
        }

        if (new[] { "премиера", "прожекция", "филм", "кино" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Cinema (очевидно)", eventName);
            return EventCategory.Cinema;
        }

        _logger.LogWarning("Could not classify '{EventName}' - needs manual categorization", eventName);
        return null;
    }

    private IEnumerable<string> ExtractMusicGenresWithKeywords(string eventName, string description)
    {
        var text = $"{eventName} {description}".ToLower();
        var genres = new List<string>();

        var genreMap = new Dictionary<string[], string>
        {
            [new[] { "slayer", "metallica", "megadeth", "anthrax", "testament", "exodus", "overkill", "kreator" }] = "thrash metal",
            [new[] { "iron maiden", "black sabbath", "judas priest", "motorhead", "dio", "ozzy osbourne", "accept" }] = "heavy metal",
            [new[] { "death", "cannibal corpse", "morbid angel", "obituary", "deicide", "suffocation", "dying fetus" }] = "death metal",
            [new[] { "mayhem", "darkthrone", "emperor", "immortal", "burzum", "bathory", "venom" }] = "black metal",
            [new[] { "helloween", "blind guardian", "stratovarius", "rhapsody", "gamma ray", "sonata arctica" }] = "power metal",
            [new[] { "dream theater", "tool", "opeth", "porcupine tree", "meshuggah", "gojira" }] = "progressive metal",
            [new[] { "nirvana", "pearl jam", "soundgarden", "alice in chains", "stone temple pilots" }] = "grunge",
            [new[] { "led zeppelin", "pink floyd", "deep purple", "queen", "the beatles", "the rolling stones" }] = "класически рок",
            [new[] { "jazz", "джаз", "coltrane", "miles davis", "herbie hancock", "keith jarrett", "pat metheny" }] = "джаз",
            [new[] { "blues", "блус", "bb king", "muddy waters", "robert johnson", "eric clapton" }] = "блус",
            [new[] { "classical", "класическа", "симфония", "оркестър", "mozart", "beethoven", "bach", "chopin" }] = "класическа",
            [new[] { "electronic", "електронна", "techno", "house", "deadmau5", "tiësto", "armin", "calvin harris" }] = "електронна",
            [new[] { "pop", "поп", "madonna", "michael jackson", "taylor swift", "ed sheeran" }] = "поп",
            [new[] { "folk", "фолк", "народна", "bob dylan", "joni mitchell", "neil young" }] = "фолк",
            [new[] { "hip-hop", "хип-хоп", "rap", "рап", "eminem", "jay-z", "kendrick lamar" }] = "хип-хоп",
            [new[] { "пп", "сигнал", "тангра", "фсб", "епизод", "диапазон", "щурците", "б.т.р.", "конкрет", "остава" }] = "българска музика"
        };

        foreach (var (keywords, genre) in genreMap)
        {
            if (keywords.Any(keyword => text.Contains(keyword)))
            {
                genres.Add(genre);
            }
        }

        if (genres.Any())
        {
            _logger.LogInformation("Detected music genres for '{EventName}': {Genres}", eventName, string.Join(", ", genres));
        }

        return genres.Any() ? genres.Take(3) : new[] { "музика" };
    }

    private List<string> ExtractTagsWithKeywords(string eventName, string description, string? location, EventCategory? category)
    {
        var tags = new List<string>();
        var text = $"{eventName} {description}".ToLower();

        // Location
        tags.Add(location?.ToLower() ?? "софия");

        // Category-specific enhanced tags
        switch (category)
        {
            case EventCategory.Music:
                if (text.Contains("slayer"))
                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "легендарни", "концерт" });
                else if (text.Contains("metallica"))
                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "мегазвезди", "концерт" });
                else if (text.Contains("iron maiden"))
                    tags.AddRange(new[] { "heavy metal", "nwobhm", "международни", "класически", "концерт" });
                else if (text.Contains("megadeth"))
                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "концерт" });
                else if (text.Contains("jazz") || text.Contains("джаз"))
                    tags.AddRange(new[] { "джаз", "импровизация", "sophisticated", "концерт" });
                else if (text.Contains("blues") || text.Contains("блус"))
                    tags.AddRange(new[] { "блус", "традиционна музика", "концерт" });
                else if (text.Contains("класическа") || text.Contains("симфония"))
                    tags.AddRange(new[] { "класическа музика", "оркестър", "концерт" });
                else if (text.Contains("концерт"))
                    tags.Add("концерт");

                if (new[] { "пп", "сигнал", "тангра", "фсб", "щурците" }.Any(keyword => text.Contains(keyword)))
                    tags.Add("българска музика");
                break;

            case EventCategory.Sports:
                if (text.Contains("футбол")) tags.Add("футбол");
                if (text.Contains("баскетбол")) tags.Add("баскетбол");
                if (text.Contains("тенис")) tags.Add("тенис");
                if (text.Contains("волейбол")) tags.Add("волейбол");
                break;

            case EventCategory.Theatre:
                if (text.Contains("драма")) tags.Add("драма");
                if (text.Contains("комедия")) tags.Add("комедия");
                if (text.Contains("мюзикъл")) tags.Add("мюзикъл");
                break;

            case null:
                // Някакви общи тагове за некласифицирани събития
                tags.Add("некласифицирано");
                break;
        }

        // General enhanced tags
        if (text.Contains("безплатно") || text.Contains("free")) tags.Add("безплатно");
        if (text.Contains("вечер") || text.Contains("evening") || text.Contains("нощ")) tags.Add("вечерно");
        if (text.Contains("сутрин") || text.Contains("утро") || text.Contains("morning")) tags.Add("сутрешно");
        if (text.Contains("семейство") || text.Contains("family") || text.Contains("деца")) tags.Add("семейно");
        if (text.Contains("открито") || text.Contains("outdoor") || text.Contains("парк")) tags.Add("на открито");
        if (text.Contains("international") || text.Contains("международн")) tags.Add("международни");

        // Ensure minimum tags
        if (tags.Count == 1) tags.Add("събитие");

        return tags.Distinct().Take(6).ToList();
    }

    private TaggingResult GenerateEnhancedFallbackTags(string eventName, string description, string? location)
    {
        var category = TryClassifyWithKeywords(eventName, description);
        var tags = ExtractTagsWithKeywords(eventName, description, location, category);

        // Extract subcategory for fallback
        string? suggestedSubCategory = null;
        if (category.HasValue)
        {
            suggestedSubCategory = ExtractSubCategory(eventName, description, category.Value);
        }

        // Add music genres if it's a music event
        if (category == EventCategory.Music)
        {
            var musicGenres = ExtractMusicGenresWithKeywords(eventName, description);
            tags.AddRange(musicGenres);
            tags = tags.Distinct().Take(6).ToList();
        }

        return new TaggingResult
        {
            SuggestedCategory = category,
            SuggestedSubCategory = suggestedSubCategory,
            SuggestedTags = tags,
            Confidence = tags.ToDictionary(tag => tag, _ => 0.8)
        };
    }

    private string? ExtractSubCategory(string eventName, string description, EventCategory category)
    {
        var text = $"{eventName} {description}".ToLower();

        return category switch
        {
            EventCategory.Music => ExtractMusicSubCategory(text),
            EventCategory.Sports => ExtractSportsSubCategory(text),
            _ => null // TODO: add other Subcategories
        };
    }

    private string? ExtractMusicSubCategory(string text)
    {
        // Metal detection with high priority
        if (new[] { "slayer", "metallica", "megadeth", "anthrax", "iron maiden", "black sabbath" }.Any(keyword => text.Contains(keyword)))
            return "Metal";

        if (new[] { "jazz", "джаз", "coltrane", "miles davis" }.Any(keyword => text.Contains(keyword)))
            return "Jazz";

        if (new[] { "rock", "рок", "led zeppelin", "pink floyd" }.Any(keyword => text.Contains(keyword)))
            return "Rock";

        if (new[] { "classical", "класическа", "symphony", "симфония", "orchestra", "оркестър" }.Any(keyword => text.Contains(keyword)))
            return "Classical";

        if (new[] { "electronic", "електронна", "techno", "house", "dj" }.Any(keyword => text.Contains(keyword)))
            return "Electronic";

        if (new[] { "folk", "фолк", "народна" }.Any(keyword => text.Contains(keyword)))
            return "Folk";

        if (new[] { "blues", "блус" }.Any(keyword => text.Contains(keyword)))
            return "Blues";

        if (new[] { "pop", "поп" }.Any(keyword => text.Contains(keyword)))
            return "Pop";

        if (new[] { "hip-hop", "хип-хоп", "rap", "рап" }.Any(keyword => text.Contains(keyword)))
            return "HipHop";

        return null;
    }

    private string? ExtractSportsSubCategory(string text)
    {
        if (new[] { "футбол", "football" }.Any(keyword => text.Contains(keyword)))
            return "Football";

        if (new[] { "баскетбол", "basketball" }.Any(keyword => text.Contains(keyword)))
            return "Basketball";

        if (new[] { "тенис", "tennis" }.Any(keyword => text.Contains(keyword)))
            return "Tennis";

        if (new[] { "волейбол", "volleyball" }.Any(keyword => text.Contains(keyword)))
            return "Volleyball";

        if (new[] { "плуване", "swimming" }.Any(keyword => text.Contains(keyword)))
            return "Swimming";

        if (new[] { "атлетика", "athletics" }.Any(keyword => text.Contains(keyword)))
            return "Athletics";

        return null;
    }

    private async Task EnsureRateLimit()
    {
        // Use Task.Run for the synchronous lock operation
        await Task.Run(() =>
        {
            lock (_lockObject)
            {
                var timeSinceLastRequest = DateTime.UtcNow - _lastRequest;
                if (timeSinceLastRequest < TimeSpan.FromMilliseconds(500)) // 500ms delay
                {
                    var delay = TimeSpan.FromMilliseconds(500) - timeSinceLastRequest;
                    if (delay > TimeSpan.Zero)
                    {
                        Task.Delay(delay).Wait();
                    }
                }
                _lastRequest = DateTime.UtcNow;
            }
        });
    }
}

// Response models for Groq
public class GroqResponse
{
    [JsonPropertyName("choices")]
    public GroqChoice[]? Choices { get; set; }
}

public class GroqChoice
{
    [JsonPropertyName("message")]
    public GroqMessage? Message { get; set; }
}

public class GroqMessage
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class PreciseAiTaggingResponse
{
    [JsonPropertyName("category")]
    public int Category { get; set; }

    [JsonPropertyName("subcategory")]
    public string? Subcategory { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
}
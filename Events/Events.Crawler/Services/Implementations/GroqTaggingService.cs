using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class GroqTaggingService : IAiTaggingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<GroqTaggingService> _logger;
    private static readonly SemaphoreSlim _rateLimiter = new(1, 1); // Reduced Max 1 concurrent request for safety
    private static DateTime _lastRequest = DateTime.MinValue;
    private static readonly object _lockObject = new();
    private static int _consecutiveFailures = 0; // Track consecutive failures

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
        // Use the comprehensive method instead
        var result = await ProcessEventComprehensivelyAsync(eventName, description);
        return result.SuggestedCategory;
    }

    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
    {
        return await ProcessEventComprehensivelyAsync(eventName, description, location);
    }

    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
    {
        return await Task.FromResult(ExtractMusicGenresWithKeywords(eventName, description));
    }

    // Single comprehensive method - replaces both ClassifyEventAsync + GenerateTagsAsync
    public async Task<TaggingResult> ProcessEventComprehensivelyAsync(string eventName, string description, string? location = null)
    {
        // Skip AI if we've had too many consecutive failures
        if (_consecutiveFailures >= 3)
        {
            _logger.LogWarning("Skipping AI for '{EventName}' due to {Failures} consecutive failures - using fallback", eventName, _consecutiveFailures);
            return GenerateEnhancedFallbackTags(eventName, description, location);
        }

        await _rateLimiter.WaitAsync();
        try
        {
            await EnsureRateLimit();

            // Complete prompt with ALL subcategories - optimized for token efficiency
            var prompt = $@"Event: {eventName}
Desc: {description}

Categories & Subcategories:
1=Music → Rock,Jazz,Metal,Pop,Classical,Electronic,Folk,Blues,HipHop,Other
2=Art → Painting,Sculpture,Photography,DigitalArt,Other
3=Business → NetworkingEvents,Startups,Marketing,Other
4=Sports → Football,Basketball,Tennis,Volleyball,Other
5=Theatre → Drama,Comedy,MusicalTheatre,Other
6=Cinema → FeatureFilms,Documentaries,Animation,Other
7=Festivals → MusicFestivals,FilmFestivals,ArtFestivals,FoodAndWineFestivals,Other
8=Exhibitions → ArtExhibitions,PhotographyExhibitions,HistoricalExhibitions,Other
9=Conferences → TechConferences,BusinessConferences,AcademicConferences,Other
10=Workshops → ArtWorkshops,CookingWorkshops,CodingWorkshops,Other
11=Undefined

Examples:
""Slayer концерт"" → ""1|Metal|thrash metal,софия,концерт""
""Левски-ЦСКА"" → ""4|Football|футбол,софия,спорт""
""Пикасо изложба"" → ""8|ArtExhibitions|изкуство,софия,живопис""
""София филм фест"" → ""7|FilmFestivals|филми,софия,фестивал""

CRITICAL: Return ONLY ""CATEGORY|SUBCATEGORY|tag1,tag2,tag3"" format!
NO explanations! NO descriptions! ONLY the format!

Return:";

            var requestBody = new
            {
                messages = new[] { new { role = "user", content = prompt } },
                model = "llama-3.1-8b-instant",
                max_tokens = 50,
                temperature = 0.1
            };

            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var responseText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

                if (!string.IsNullOrEmpty(responseText))
                {
                    _logger.LogInformation("AI comprehensive response for '{EventName}': {Response}", eventName, responseText);

                    // Strict validation: Check if response follows format
                    if (!IsValidResponseFormat(responseText))
                    {
                        _logger.LogWarning("AI returned invalid format for '{EventName}': {Response} - using fallback", eventName, responseText);
                        _consecutiveFailures++; // Count as failure
                        return GenerateEnhancedFallbackTags(eventName, description, location);
                    }

                    var parts = responseText.Split('|');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out var categoryId))
                    {
                        if (categoryId == 11)
                        {
                            _logger.LogWarning("AI classified '{EventName}' as Undefined", eventName);
                            _consecutiveFailures = Math.Max(0, _consecutiveFailures - 1); // Partial success
                            return GenerateEnhancedFallbackTags(eventName, description, location);
                        }

                        if (Enum.IsDefined(typeof(EventCategory), categoryId))
                        {
                            var category = (EventCategory)categoryId;
                            var subcategory = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1].Trim() : "Other";

                            // Extract tags from AI response or use fallback
                            var aiTags = parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2])
                                ? parts[2].Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList()
                                : new List<string>();

                            // Combine AI tags with keyword-extracted tags
                            var keywordTags = ExtractTagsWithKeywords(eventName, description, location, category);
                            var allTags = aiTags.Concat(keywordTags).Distinct().Take(6).ToList();

                            _logger.LogInformation("AI classified '{EventName}' as {Category}|{SubCategory} with tags: {Tags}",
                                eventName, category, subcategory, string.Join(", ", allTags));

                            // Reset failure counter
                            _consecutiveFailures = 0;

                            return new TaggingResult
                            {
                                SuggestedCategory = category,
                                SuggestedSubCategory = subcategory,
                                SuggestedTags = allTags,
                                Confidence = allTags.ToDictionary(tag => tag, _ => 0.85)
                            };
                        }
                    }
                }

                _logger.LogWarning("AI returned invalid response: '{Response}' for event '{EventName}'", responseText, eventName);
                _consecutiveFailures++;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("AI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                _consecutiveFailures++;
            }

            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("rate_limit_exceeded") || ex.Message.Contains("TooManyRequests"))
        {
            _consecutiveFailures++;
            _logger.LogWarning("Rate limit exceeded (failure #{Failures}), using fallback classification for '{EventName}'",
                _consecutiveFailures, eventName);
            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            _logger.LogError(ex, "Error in comprehensive AI processing for '{EventName}' (failure #{Failures})", eventName, _consecutiveFailures);
            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    // Strict format validation
    private static bool IsValidResponseFormat(string response)
    {
        // Must match: NUMBER|WORD|word,word,word format
        var pattern = @"^\d+\|[A-Za-z]*\|[a-zA-Zа-я\s,]*$";
        return Regex.IsMatch(response, pattern);
    }

    private EventCategory? TryClassifyWithKeywords(string eventName, string description)
    {
        var text = $"{eventName} {description}".ToLower();

        if (new[] { "концерт", "concert", "live music", "band", "група" }.Any(keyword => text.Contains(keyword)) ||
            new[] { "slayer", "metallica", "iron maiden", "megadeth", "jazz", "джаз" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Music", eventName);
            return EventCategory.Music;
        }

        if (new[] { "мач", "дерби", "турнир", "състезание", "футбол", "баскетбол" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Sports", eventName);
            return EventCategory.Sports;
        }

        if (new[] { "театър", "пиеса", "спектакъл", "драма", "комедия" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Theatre", eventName);
            return EventCategory.Theatre;
        }

        if (new[] { "премиера", "прожекция", "филм", "кино" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Cinema", eventName);
            return EventCategory.Cinema;
        }

        if (new[] { "изложба", "галерия", "експозиция" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Exhibitions", eventName);
            return EventCategory.Exhibitions;
        }

        if (new[] { "фестивал", "фест", "празник" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Festivals", eventName);
            return EventCategory.Festivals;
        }

        if (new[] { "конференция", "семинар", "лекция" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Conferences", eventName);
            return EventCategory.Conferences;
        }

        if (new[] { "работилница", "курс", "обучение", "workshop" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Workshops", eventName);
            return EventCategory.Workshops;
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
            [new[] { "iron maiden", "black sabbath", "judas priest", "motorhead", "dio", "ozzy осбърн", "accept" }] = "heavy metal",
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

        // Location - normalize common issues
        var normalizedLocation = NormalizeLocation(location);
        if (!string.IsNullOrWhiteSpace(normalizedLocation))
        {
            tags.Add(normalizedLocation);
        }

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

            case EventCategory.Cinema:
                if (text.Contains("филм")) tags.Add("филм");
                if (text.Contains("документален")) tags.Add("документален");
                if (text.Contains("анимация")) tags.Add("анимация");
                break;

            case EventCategory.Exhibitions:
                if (text.Contains("изложба")) tags.Add("изложба");
                if (text.Contains("галерия")) tags.Add("галерия");
                break;

            case EventCategory.Festivals:
                if (text.Contains("фестивал")) tags.Add("фестивал");
                if (text.Contains("празник")) tags.Add("празник");
                break;

            case null:
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

        return CleanAndValidateTags(tags).Take(6).ToList();
    }

    private string NormalizeLocation(string? location)
    {
        if (string.IsNullOrWhiteSpace(location)) return "софия";

        var normalized = location.ToLower().Trim();

        // Common location normalizations to fix the garbage tags
        var locationMap = new Dictionary<string, string>
        {
            ["sofia"] = "софия",
            ["bulgaria"] = "българия",
            ["ндк"] = "ndk",
            ["национален дворец на културата"] = "ndk",
            ["зала 1"] = "зала1"
        };

        foreach (var (key, value) in locationMap)
        {
            normalized = normalized.Replace(key, value);
        }

        // Remove addresses and keep only venue names
        if (normalized.Contains("ул.") || normalized.Contains("улица") || normalized.Contains("№"))
        {
            // Extract venue name before address
            var parts = normalized.Split(new[] { "ул.", "улица", "№", "(", "," }, StringSplitOptions.RemoveEmptyEntries);
            normalized = parts.FirstOrDefault()?.Trim() ?? "софия";
        }

        // Handle the specific garbage case: "софия, ул. "емил берсински" №5, sofia, bulgaria топлоцентрала зала 1"
        if (normalized.Contains("топлоцентрала"))
        {
            return "топлоцентрала";
        }

        if (normalized.Contains("bunker"))
        {
            return "bunker club";
        }

        return normalized.Length > 20 ? "софия" : normalized;
    }

    private List<string> CleanAndValidateTags(List<string> tags)
    {
        var cleaned = new List<string>();

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag)) continue;

            var cleanTag = tag.Trim().ToLower();

            // Skip if too long or has invalid characters
            if (cleanTag.Length > 30 || cleanTag.Contains("№") || cleanTag.Contains("ул.")) continue;

            // Skip if it's just numbers or gibberish
            if (cleanTag.All(char.IsDigit) || cleanTag.Length < 3) continue;

            cleaned.Add(cleanTag);
        }

        return cleaned.Distinct().ToList();
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
            tags = CleanAndValidateTags(tags).Take(6).ToList();
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
            EventCategory.Cinema => ExtractCinemaSubCategory(text),
            EventCategory.Festivals => ExtractFestivalsSubCategory(text),
            EventCategory.Exhibitions => ExtractExhibitionsSubCategory(text),
            _ => "Other"
        };
    }

    private string? ExtractMusicSubCategory(string text)
    {
        // Metal detection with high priority
        if (new[] { "slayer", "metallica", "megadeth", "anthrax", "iron maiden", "black sabbath" }.Any(keyword => text.Contains(keyword)))
            return "Metal";

        if (new[] { "jazz", "джаз", "coltrane", "miles davis" }.Any(keyword => text.Contains(keyword)))
            return "Jazz";

        if (new[] { "rock", "рок", "led zeppelin", "pink floyd" }.Any(keyword => text.Contains(keyword))
            || new[] { "slayer", "metallica", "megadeth" }.Any(keyword => text.Contains(keyword)))
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

        return "Other";
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

        return "Other";
    }

    private string? ExtractCinemaSubCategory(string text)
    {
        if (new[] { "документален", "documentary" }.Any(keyword => text.Contains(keyword)))
            return "Documentaries";

        if (new[] { "анимация", "animation" }.Any(keyword => text.Contains(keyword)))
            return "Animation";

        return "FeatureFilms";
    }

    private string? ExtractFestivalsSubCategory(string text)
    {
        if (new[] { "музикален", "music" }.Any(keyword => text.Contains(keyword)))
            return "MusicFestivals";

        if (new[] { "филм", "кино", "film" }.Any(keyword => text.Contains(keyword)))
            return "FilmFestivals";

        if (new[] { "храна", "вино", "food", "wine" }.Any(keyword => text.Contains(keyword)))
            return "FoodAndWineFestivals";

        return "Other";
    }

    private string? ExtractExhibitionsSubCategory(string text)
    {
        if (new[] { "картини", "живопис", "art" }.Any(keyword => text.Contains(keyword)))
            return "ArtExhibitions";

        if (new[] { "фотография", "photo" }.Any(keyword => text.Contains(keyword)))
            return "PhotographyExhibitions";

        if (new[] { "история", "исторически", "historical" }.Any(keyword => text.Contains(keyword)))
            return "HistoricalExhibitions";

        return "Other";
    }

    private async Task EnsureRateLimit()
    {
        // Use Task.Run for the synchronous lock operation
        await Task.Run(() =>
        {
            lock (_lockObject)
            {
                var timeSinceLastRequest = DateTime.UtcNow - _lastRequest;

                // More aggressive rate limiting based on failure count
                var requiredDelay = _consecutiveFailures > 0
                    ? TimeSpan.FromMilliseconds(2000 + (_consecutiveFailures * 1000)) // 2s + 1s per failure
                    : TimeSpan.FromMilliseconds(1000); // Normal 1s delay

                if (timeSinceLastRequest < requiredDelay)
                {
                    var delay = requiredDelay - timeSinceLastRequest;
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
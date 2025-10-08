//using Events.Crawler.Models;
//using Events.Crawler.Services.Interfaces;
//using Events.Models.Enums;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Events.Crawler.Services.Implementations;

//public class HuggingFaceTaggingService : IAiTaggingService
//{
//    private readonly HttpClient _http;
//    private readonly string _apiKey;
//    private readonly ILogger<HuggingFaceTaggingService> _logger;

//    public HuggingFaceTaggingService(HttpClient http, IConfiguration config, ILogger<HuggingFaceTaggingService> logger)
//    {
//        _http = http;
//        _apiKey = config["HuggingFace:ApiKey"] ?? throw new InvalidOperationException("HuggingFace API key not configured");
//        _logger = logger;

//        _http.BaseAddress = new Uri("https://api-inference.huggingface.co/");
//        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//    }

//    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
//    {
//        try
//        {
//            // Use a simpler text classification model
//            var text = $"{eventName}. {description}";

//            var requestBody = new
//            {
//                inputs = text,
//                parameters = new
//                {
//                    candidate_labels = new[]
//                    {
//                        "music", "art", "business", "sports", "theatre",
//                        "cinema", "festivals", "exhibitions", "conferences", "workshops"
//                    }
//                }
//            };

//            // Use zero-shot classification model
//            var response = await _http.PostAsJsonAsync("models/facebook/bart-large-mnli", requestBody);

//            if (response.IsSuccessStatusCode)
//            {
//                var result = await response.Content.ReadFromJsonAsync<HuggingFaceClassificationResponse>();
//                var topLabel = result?.Labels?.FirstOrDefault()?.ToLower();

//                // Map response to category
//                var category = MapLabelToCategory(topLabel);
//                _logger.LogInformation("HuggingFace classified '{EventName}' as {Category} (confidence: {Score})",
//                    eventName, category, result?.Scores?.FirstOrDefault() ?? 0);
//                return category;
//            }
//            else
//            {
//                var errorContent = await response.Content.ReadAsStringAsync();
//                _logger.LogWarning("HuggingFace API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
//                return ClassifyWithKeywords(eventName, description);
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error with HuggingFace classification for '{EventName}'", eventName);
//            return ClassifyWithKeywords(eventName, description);
//        }
//    }

//    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
//    {
//        try
//        {
//            // Use classification + enhanced keyword extraction
//            var category = await ClassifyEventAsync(eventName, description);
//            var tags = ExtractTagsWithKeywords(eventName, description, location, category);
//            var musicGenres = category == EventCategory.Music
//                ? await ExtractMusicGenresAsync(eventName, description)
//                : Enumerable.Empty<string>();

//            // Combine tags with music genres
//            var allTags = tags.Concat(musicGenres).Distinct().Take(6).ToList();

//            return new TaggingResult
//            {
//                SuggestedCategory = category,
//                SuggestedTags = allTags,
//                Confidence = allTags.ToDictionary(tag => tag, _ => 0.8)
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error generating tags with HuggingFace for '{EventName}'", eventName);
//            return GenerateFallbackTags(eventName, description);
//        }
//    }

//    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
//    {
//        var text = $"{eventName} {description}".ToLower();
//        var genres = new List<string>();

//        // Enhanced music genre detection with more bands
//        var genreMap = new Dictionary<string[], string>
//        {
//            // Thrash Metal
//            [new[] { "slayer", "metallica", "megadeth", "anthrax", "testament", "exodus" }] = "thrash metal",

//            // Heavy Metal
//            [new[] { "iron maiden", "black sabbath", "judas priest", "motorhead", "dio" }] = "heavy metal",

//            // Death Metal
//            [new[] { "death", "cannibal corpse", "morbid angel", "obituary", "deicide" }] = "death metal",

//            // Black Metal
//            [new[] { "mayhem", "darkthrone", "emperor", "immortal", "burzum" }] = "black metal",

//            // Power Metal
//            [new[] { "helloween", "blind guardian", "stratovarius", "rhapsody", "gamma ray" }] = "power metal",

//            // Progressive Metal
//            [new[] { "dream theater", "tool", "opeth", "porcupine tree" }] = "progressive metal",

//            // Jazz
//            [new[] { "jazz", "джаз", "coltrane", "miles davis", "herbie hancock", "keith jarrett" }] = "джаз",

//            // Blues
//            [new[] { "blues", "блус", "bb king", "muddy waters", "robert johnson" }] = "блус",

//            // Classical
//            [new[] { "classical", "класическа", "симфония", "оркестър", "mozart", "beethoven", "bach" }] = "класическа",

//            // Electronic
//            [new[] { "electronic", "електронна", "techno", "house", "deadmau5", "tiësto", "armin" }] = "електронна",

//            // Rock
//            [new[] { "rock", "рок", "led zeppelin", "pink floyd", "the beatles" }] = "рок",

//            // Pop
//            [new[] { "pop", "поп", "madonna", "michael jackson" }] = "поп",

//            // Folk
//            [new[] { "folk", "фолк", "народна", "bob dylan", "joni mitchell" }] = "фолк",

//            // Bulgarian artists
//            [new[] { "пп", "сигнал", "тангра", "фсб", "епизод", "диапазон", "щурците", "б.т.р." }] = "българска музика"
//        };

//        foreach (var (keywords, genre) in genreMap)
//        {
//            if (keywords.Any(keyword => text.Contains(keyword)))
//            {
//                genres.Add(genre);
//            }
//        }

//        // Log the detected genres
//        if (genres.Any())
//        {
//            _logger.LogInformation("Detected music genres for '{EventName}': {Genres}", eventName, string.Join(", ", genres));
//        }

//        return genres.Any() ? genres.Take(3) : new[] { "музика" };
//    }

//    private EventCategory MapLabelToCategory(string? label)
//    {
//        if (string.IsNullOrEmpty(label)) return EventCategory.Music;

//        return label.ToLower() switch
//        {
//            "music" => EventCategory.Music,
//            "art" => EventCategory.Art,
//            "business" => EventCategory.Business,
//            "sports" => EventCategory.Sports,
//            "theatre" => EventCategory.Theatre,
//            "cinema" => EventCategory.Cinema,
//            "festivals" => EventCategory.Festivals,
//            "exhibitions" => EventCategory.Exhibitions,
//            "conferences" => EventCategory.Conferences,
//            "workshops" => EventCategory.Workshops,
//            _ => EventCategory.Music
//        };
//    }

//    private EventCategory ClassifyWithKeywords(string eventName, string description)
//    {
//        var text = $"{eventName} {description}".ToLower();

//        // Enhanced music detection with more keywords
//        var musicKeywords = new[]
//        {
//            // General
//            "концерт", "band", "група", "музика", "song", "песен", "album", "албум", "изпълнител", "певец", "певица",
            
//            // Metal bands (international)
//            "slayer", "metallica", "iron maiden", "megadeth", "anthrax", "black sabbath", "judas priest",
//            "testament", "exodus", "death", "cannibal corpse", "mayhem", "darkthrone", "dream theater",
            
//            // Bulgarian bands
//            "пп", "сигнал", "тангра", "фсб", "епизод", "диапазон", "щурците", "б.т.р.", "конкрет", "остава",
            
//            // Genres
//            "metal", "метъл", "rock", "рок", "jazz", "джаз", "pop", "поп", "електронна", "класическа",
//            "folk", "фолк", "blues", "блус", "techno", "house", "hip-hop", "хип-хоп",
            
//            // Venues
//            "клуб", "club", "бар", "bar", "сцена", "stage", "зала", "hall"
//        };

//        if (musicKeywords.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Music;

//        // Other categories with enhanced keywords
//        if (new[] { "театър", "пиеса", "спектакъл", "актьор", "актриса", "драма", "комедия" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Theatre;

//        if (new[] { "спорт", "футбол", "баскетбол", "тенис", "мач", "игра", "турнир", "състезание" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Sports;

//        if (new[] { "изложба", "галерия", "картина", "изкуство", "художник", "скулптура" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Art;

//        if (new[] { "кино", "филм", "прожекция", "cinema", "movie", "премиера" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Cinema;

//        if (new[] { "фестивал", "празник", "festival", "празненство" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Festivals;

//        if (new[] { "конференция", "семинар", "форум", "дискусия", "лекция" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Conferences;

//        if (new[] { "работилница", "курс", "обучение", "workshop", "тренинг" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Workshops;

//        if (new[] { "бизнес", "networking", "предприемачество", "стартъп", "startup" }.Any(keyword => text.Contains(keyword)))
//            return EventCategory.Business;

//        return EventCategory.Music; // Default for Sofia (most events are music)
//    }

//    private List<string> ExtractTagsWithKeywords(string eventName, string description, string? location, EventCategory category)
//    {
//        var tags = new List<string>();
//        var text = $"{eventName} {description}".ToLower();

//        // Location
//        tags.Add(location?.ToLower() ?? "софия");

//        // Category-specific enhanced tags
//        switch (category)
//        {
//            case EventCategory.Music:
//                // Specific metal subgenres
//                if (text.Contains("slayer") || text.Contains("metallica") || text.Contains("megadeth"))
//                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни изпълнители" });
//                else if (text.Contains("iron maiden") || text.Contains("black sabbath"))
//                    tags.AddRange(new[] { "heavy metal", "класически метъл" });
//                else if (text.Contains("jazz") || text.Contains("джаз"))
//                    tags.AddRange(new[] { "джаз", "импровизация" });
//                else if (text.Contains("blues") || text.Contains("блус"))
//                    tags.AddRange(new[] { "блус", "традиционна музика" });
//                else if (text.Contains("класическа") || text.Contains("симфония"))
//                    tags.AddRange(new[] { "класическа музика", "оркестър" });
//                else if (text.Contains("концерт"))
//                    tags.Add("концерт");

//                // Bulgarian music
//                if (new[] { "пп", "сигнал", "тангра", "фсб", "щурците" }.Any(keyword => text.Contains(keyword)))
//                    tags.Add("българска музика");

//                break;

//            case EventCategory.Sports:
//                if (text.Contains("футбол")) tags.Add("футбол");
//                if (text.Contains("баскетбол")) tags.Add("баскетбол");
//                if (text.Contains("тенис")) tags.Add("тенис");
//                break;

//            case EventCategory.Theatre:
//                if (text.Contains("драма")) tags.Add("драма");
//                if (text.Contains("комедия")) tags.Add("комедия");
//                break;
//        }

//        // General enhanced tags
//        if (text.Contains("безплатно") || text.Contains("free")) tags.Add("безплатно");
//        if (text.Contains("вечер") || text.Contains("evening") || text.Contains("нощ")) tags.Add("вечерно");
//        if (text.Contains("сутрин") || text.Contains("утро") || text.Contains("morning")) tags.Add("сутрешно");
//        if (text.Contains("семейство") || text.Contains("family") || text.Contains("деца")) tags.Add("семейно");
//        if (text.Contains("открито") || text.Contains("outdoor") || text.Contains("парк")) tags.Add("на открито");
//        if (text.Contains("international") || text.Contains("международн")) tags.Add("международни");

//        // Ensure minimum tags
//        if (tags.Count == 1) tags.Add("събитие");

//        return tags.Distinct().Take(6).ToList();
//    }

//    private TaggingResult GenerateFallbackTags(string eventName, string description)
//    {
//        var category = ClassifyWithKeywords(eventName, description);
//        var tags = ExtractTagsWithKeywords(eventName, description, null, category);

//        return new TaggingResult
//        {
//            SuggestedCategory = category,
//            SuggestedTags = tags,
//            Confidence = tags.ToDictionary(tag => tag, _ => 0.6)
//        };
//    }
//}

//// Updated response models for HuggingFace
//public class HuggingFaceClassificationResponse
//{
//    [JsonPropertyName("labels")]
//    public string[]? Labels { get; set; }

//    [JsonPropertyName("scores")]
//    public double[]? Scores { get; set; }
//}

//public class HuggingFaceResponse
//{
//    [JsonPropertyName("generated_text")]
//    public string? GeneratedText { get; set; }
//}
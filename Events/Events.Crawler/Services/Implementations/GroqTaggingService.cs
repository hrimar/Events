//using Events.Crawler.Models;
//using Events.Crawler.Services.Interfaces;
//using Events.Models.Enums;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Events.Crawler.Services.Implementations;

//public class GroqTaggingService : IAiTaggingService
//{
//    private readonly HttpClient _http;
//    private readonly string _apiKey;
//    private readonly ILogger<GroqTaggingService> _logger;

//    public GroqTaggingService(HttpClient http, IConfiguration config, ILogger<GroqTaggingService> logger)
//    {
//        _http = http;
//        _apiKey = config["Groq:ApiKey"] ?? throw new InvalidOperationException("Groq API key not configured");
//        _logger = logger;

//        _http.BaseAddress = new Uri("https://api.groq.com/");
//        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//    }

//    //    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
//    //    {
//    //        try
//    //        {
//    //            var prompt = $@"Analyze this Bulgarian event and classify it. Return ONLY the number (1-10):

//    //Event: {eventName}
//    //Description: {description}

//    //Categories:
//    //1=Music (концерти, музикални изпълнения, bands)
//    //2=Art (изложби, галерии, visual arts)
//    //3=Business (networking, бизнес събития)
//    //4=Sports (спортни събития, мачове)
//    //5=Theatre (театрални постановки)
//    //6=Cinema (кино прожекции)
//    //7=Festivals (фестивали)
//    //8=Exhibitions (изложби, музеи)
//    //9=Conferences (конференции, семинари)
//    //10=Workshops (работилници, курсове)

//    //Examples:
//    //- ""Slayer концерт"" = 1
//    //- ""Джаз вечер"" = 1
//    //- ""Футболен мач"" = 4

//    //Return only the number:";

//    //            var requestBody = new
//    //            {
//    //                messages = new[]
//    //                {
//    //                    new { role = "user", content = prompt }
//    //                },
//    //                model = "llama-3.1-8b-instant", // Updated to current available model
//    //                max_tokens = 5,
//    //                temperature = 0.1
//    //            };

//    //            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

//    //            if (response.IsSuccessStatusCode)
//    //            {
//    //                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
//    //                var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//    //                if (int.TryParse(categoryText, out var categoryId) &&
//    //                    Enum.IsDefined(typeof(EventCategory), categoryId))
//    //                {
//    //                    _logger.LogInformation("Groq classified '{EventName}' as {Category}", eventName, (EventCategory)categoryId);
//    //                    return (EventCategory)categoryId;
//    //                }
//    //                else
//    //                {
//    //                    _logger.LogWarning("Groq returned non-numeric response: '{Response}' for event '{EventName}'", categoryText, eventName);
//    //                }
//    //            }
//    //            else
//    //            {
//    //                var errorContent = await response.Content.ReadAsStringAsync();
//    //                _logger.LogWarning("Groq API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
//    //            }

//    //            // Fallback to keyword classification
//    //            return ClassifyWithKeywords(eventName, description);
//    //        }
//    //        catch (Exception ex)
//    //        {
//    //            _logger.LogError(ex, "Error with Groq classification for '{EventName}'", eventName);
//    //            return ClassifyWithKeywords(eventName, description);
//    //        }
//    //    }
//    public async Task<EventCategory?> ClassifyEventAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $@"Analyze this Bulgarian event and classify it. Return ONLY the number (1-10):

//Event: {eventName}
//Description: {description}

//Categories:
//1=Music (концерти, музикални изпълнения, bands, DJ sets, festivals with music)
//2=Art (изложби, галерии, visual arts, художествени инсталации)
//3=Business (networking, бизнес събития, предприемачество, стартъп събития)
//4=Sports (спортни събития, мачове, турнири, състезания)
//5=Theatre (театрални постановки, драма, комедия, мюзикъли)
//6=Cinema (кино прожекции, филми, премиери, документални)
//7=Festivals (многодневни фестивали, културни празници, тематични събития)
//8=Exhibitions (изложби, музеи, исторически експозиции)
//9=Conferences (конференции, семинари, лекции, научни форуми)
//10=Workshops (работилници, курсове, обучения, майсторски класове)

//Detailed Examples:
//- ""Slayer концерт в Зала 1"" = 1 (Music - metal concert)
//- ""Iron Maiden live in Sofia"" = 1 (Music - rock concert)
//- ""Джаз вечер с John Coltrane Tribute"" = 1 (Music - jazz performance)
//- ""Изложба на Пикасо в НХГ"" = 2 (Art - painting exhibition)
//- ""StartUp Bulgaria 2024"" = 3 (Business - startup event)
//- ""Левски - ЦСКА дерби"" = 4 (Sports - football match)
//- ""Хамлет в Народния театър"" = 5 (Theatre - drama performance)
//- ""Премиера на Дюн 2"" = 6 (Cinema - movie premiere)
//- ""Панаир на книгата София"" = 7 (Festival - cultural festival)
//- ""Изложба Тракийско злато"" = 8 (Exhibition - historical exhibition)
//- ""TEDx Sofia 2024"" = 9 (Conference - technology conference)
//- ""UX/UI Design Workshop"" = 10 (Workshop - design training)

//If you cannot determine the category with confidence, return 0.

//Return only the number (0-10):";

//            var requestBody = new
//            {
//                messages = new[]
//                {
//                new { role = "user", content = prompt }
//            },
//                model = "llama-3.1-8b-instant",
//                max_tokens = 5,
//                temperature = 0.1
//            };

//            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

//            if (response.IsSuccessStatusCode)
//            {
//                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
//                var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//                if (int.TryParse(categoryText, out var categoryId))
//                {
//                    if (categoryId == 0)
//                    {
//                        _logger.LogWarning("AI could not classify '{EventName}' with confidence", eventName);
//                        return null; // AI није сигуран
//                    }

//                    if (Enum.IsDefined(typeof(EventCategory), categoryId))
//                    {
//                        _logger.LogInformation("AI classified '{EventName}' as {Category}", eventName, (EventCategory)categoryId);
//                        return (EventCategory)categoryId;
//                    }
//                }

//                _logger.LogWarning("AI returned invalid response: '{Response}' for event '{EventName}'", categoryText, eventName);
//            }
//            else
//            {
//                var errorContent = await response.Content.ReadAsStringAsync();
//                _logger.LogWarning("AI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
//            }

//            // Ако AI не работи, опитай keyword classification
//            return TryClassifyWithKeywords(eventName, description);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error with AI classification for '{EventName}'", eventName);
//            return TryClassifyWithKeywords(eventName, description);
//        }
//    }

//    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
//    {
//        try
//        {
//            // Use enhanced keyword-based approach + AI classification
//            var category = await ClassifyEventAsync(eventName, description);
//            var tags = ExtractTagsWithKeywords(eventName, description, location, category);
//            var musicGenres = category == EventCategory.Music
//                ? ExtractMusicGenresWithKeywords(eventName, description)
//                : Enumerable.Empty<string>();

//            // Combine tags with music genres
//            var allTags = tags.Concat(musicGenres).Distinct().Take(6).ToList();

//            _logger.LogInformation("Generated comprehensive tags for '{EventName}': Category={Category}, Tags=[{Tags}]",
//                eventName, category, string.Join(", ", allTags));

//            return new TaggingResult
//            {
//                SuggestedCategory = category,
//                SuggestedTags = allTags,
//                Confidence = allTags.ToDictionary(tag => tag, _ => 0.8)
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error generating tags with Groq for '{EventName}'", eventName);
//            return GenerateEnhancedFallbackTags(eventName, description, location);
//        }
//    }

//    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
//    {
//        return ExtractMusicGenresWithKeywords(eventName, description);
//    }

//    private EventCategory? TryClassifyWithKeywords(string eventName, string description)
//    {
//        var text = $"{eventName} {description}".ToLower();

//        // Само много очевидни случаи - НЕ default!

//        // Music - само ако е много очевидно
//        if (new[] { "концерт", "concert", "live music", "band", "група" }.Any(keyword => text.Contains(keyword)) ||
//            new[] { "slayer", "metallica", "iron maiden", "megadeth", "jazz", "джаз" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Keyword classified '{EventName}' as Music (очевидно)", eventName);
//            return EventCategory.Music;
//        }

//        // Sports - само ако е много очевидно
//        if (new[] { "мач", "дерби", "турнир", "състезание", "футбол", "баскетбол" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Keyword classified '{EventName}' as Sports (очевидно)", eventName);
//            return EventCategory.Sports;
//        }

//        // Theatre - само ако е много очевидно
//        if (new[] { "театър", "пиеса", "спектакъл", "драма", "комедия" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Keyword classified '{EventName}' as Theatre (очевидно)", eventName);
//            return EventCategory.Theatre;
//        }

//        // Cinema - само ако е много очевидно
//        if (new[] { "премиера", "прожекция", "филм", "кино" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Keyword classified '{EventName}' as Cinema (очевидно)", eventName);
//            return EventCategory.Cinema;
//        }

//        // Ако не е очевидно - НЕ гадаем!
//        _logger.LogWarning("Could not classify '{EventName}' - needs manual categorization", eventName);
//        return null;
//    }

//    private IEnumerable<string> ExtractMusicGenresWithKeywords(string eventName, string description)
//    {
//        var text = $"{eventName} {description}".ToLower();
//        var genres = new List<string>();

//        // Very comprehensive music genre detection
//        var genreMap = new Dictionary<string[], string>
//        {
//            // Thrash Metal - The Big Four + more
//            [new[] { "slayer", "metallica", "megadeth", "anthrax", "testament", "exodus", "overkill", "kreator" }] = "thrash metal",

//            // Heavy Metal classics
//            [new[] { "iron maiden", "black sabbath", "judas priest", "motorhead", "dio", "ozzy osbourne", "accept" }] = "heavy metal",

//            // Death Metal
//            [new[] { "death", "cannibal corpse", "morbid angel", "obituary", "deicide", "suffocation", "dying fetus" }] = "death metal",

//            // Black Metal
//            [new[] { "mayhem", "darkthrone", "emperor", "immortal", "burzum", "bathory", "venom" }] = "black metal",

//            // Power Metal
//            [new[] { "helloween", "blind guardian", "stratovarius", "rhapsody", "gamma ray", "sonata arctica" }] = "power metal",

//            // Progressive Metal/Rock
//            [new[] { "dream theater", "tool", "opeth", "porcupine tree", "meshuggah", "gojira" }] = "progressive metal",

//            // Alternative/Grunge
//            [new[] { "nirvana", "pearl jam", "soundgarden", "alice in chains", "stone temple pilots" }] = "grunge",

//            // Classic Rock
//            [new[] { "led zeppelin", "pink floyd", "deep purple", "queen", "the beatles", "the rolling stones" }] = "класически рок",

//            // Jazz
//            [new[] { "jazz", "джаз", "coltrane", "miles davis", "herbie hancock", "keith jarrett", "pat metheny" }] = "джаз",

//            // Blues
//            [new[] { "blues", "блус", "bb king", "muddy waters", "robert johnson", "eric clapton" }] = "блус",

//            // Classical
//            [new[] { "classical", "класическа", "симфония", "оркестър", "mozart", "beethoven", "bach", "chopin" }] = "класическа",

//            // Electronic/EDM
//            [new[] { "electronic", "електронна", "techno", "house", "deadmau5", "tiësto", "armin", "calvin harris" }] = "електронна",

//            // Pop
//            [new[] { "pop", "поп", "madonna", "michael jackson", "taylor swift", "ed sheeran" }] = "поп",

//            // Folk
//            [new[] { "folk", "фолк", "народна", "bob dylan", "joni mitchell", "neil young" }] = "фолк",

//            // Hip-Hop
//            [new[] { "hip-hop", "хип-хоп", "rap", "рап", "eminem", "jay-z", "kendrick lamar" }] = "хип-хоп",

//            // Bulgarian artists
//            [new[] { "пп", "сигнал", "тангра", "фсб", "епизод", "диапазон", "щурците", "б.т.р.", "конкрет", "остава" }] = "българска музика"
//        };

//        foreach (var (keywords, genre) in genreMap)
//        {
//            if (keywords.Any(keyword => text.Contains(keyword)))
//            {
//                genres.Add(genre);
//            }
//        }

//        if (genres.Any())
//        {
//            _logger.LogInformation("Detected music genres for '{EventName}': {Genres}", eventName, string.Join(", ", genres));
//        }

//        return genres.Any() ? genres.Take(3) : new[] { "музика" };
//    }

//    private EventCategory ClassifyWithKeywords(string eventName, string description)
//    {
//        var text = $"{eventName} {description}".ToLower();

//        // Enhanced music detection with specific bands and genres
//        var musicKeywords = new[]
//        {
//            // General music terms
//            "концерт", "band", "група", "музика", "song", "песен", "album", "албум", "изпълнител", "певец", "певица",

//            // Metal/Rock bands (international)
//            "slayer", "metallica", "iron maiden", "megadeth", "anthrax", "black sabbath", "judas priest",
//            "dream theater", "tool", "opeth", "led zeppelin", "pink floyd", "deep purple", "queen",

//            // Bulgarian bands
//            "пп", "сигнал", "тангра", "фсб", "епизод", "диапазон", "щурците", "б.т.р.", "конкрет", "остава",

//            // Genres
//            "metal", "метъл", "rock", "рок", "jazz", "джаз", "pop", "поп", "електронна", "класическа",
//            "folk", "фолк", "blues", "блус", "techno", "house", "hip-hop", "хип-хоп", "rap", "рап",

//            // Music venues
//            "клуб", "club", "бар", "bar", "сцена", "stage", "зала", "hall", "арена", "stadium"
//        };

//        if (musicKeywords.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Music via keywords", eventName);
//            return EventCategory.Music;
//        }

//        // Other categories with enhanced detection
//        if (new[] { "театър", "пиеса", "спектакъл", "актьор", "актриса", "драма", "комедия", "мюзикъл" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Theatre via keywords", eventName);
//            return EventCategory.Theatre;
//        }

//        if (new[] { "спорт", "футбол", "баскетбол", "тенис", "мач", "игра", "турнир", "състезание", "волейбол" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Sports via keywords", eventName);
//            return EventCategory.Sports;
//        }

//        if (new[] { "изложба", "галерия", "картина", "изкуство", "художник", "скулптура", "фотография" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Art via keywords", eventName);
//            return EventCategory.Art;
//        }

//        if (new[] { "кино", "филм", "прожекция", "cinema", "movie", "премиера", "документален" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Cinema via keywords", eventName);
//            return EventCategory.Cinema;
//        }

//        if (new[] { "фестивал", "празник", "festival", "празненство", "карнавал" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Festivals via keywords", eventName);
//            return EventCategory.Festivals;
//        }

//        if (new[] { "конференция", "семинар", "форум", "дискусия", "лекция", "презентация" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Conferences via keywords", eventName);
//            return EventCategory.Conferences;
//        }

//        if (new[] { "работилница", "курс", "обучение", "workshop", "тренинг", "майсторски клас" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Workshops via keywords", eventName);
//            return EventCategory.Workshops;
//        }

//        if (new[] { "бизнес", "networking", "предприемачество", "стартъп", "startup", "инвестиции" }.Any(keyword => text.Contains(keyword)))
//        {
//            _logger.LogInformation("Classified '{EventName}' as Business via keywords", eventName);
//            return EventCategory.Business;
//        }

//        _logger.LogInformation("Classified '{EventName}' as Music (default for Sofia)", eventName);
//        return EventCategory.Music; // Default for Sofia
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
//                // Specific band/artist detection with precise tagging
//                if (text.Contains("slayer"))
//                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "легендарни", "концерт" });
//                else if (text.Contains("metallica"))
//                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "мегазвезди", "концерт" });
//                else if (text.Contains("iron maiden"))
//                    tags.AddRange(new[] { "heavy metal", "nwobhm", "международни", "класически", "концерт" });
//                else if (text.Contains("megadeth"))
//                    tags.AddRange(new[] { "thrash metal", "heavy metal", "международни", "концерт" });
//                else if (text.Contains("jazz") || text.Contains("джаз"))
//                    tags.AddRange(new[] { "джаз", "импровизация", "sophisticated", "концерт" });
//                else if (text.Contains("blues") || text.Contains("блус"))
//                    tags.AddRange(new[] { "блус", "традиционна музика", "концерт" });
//                else if (text.Contains("класическа") || text.Contains("симфония"))
//                    tags.AddRange(new[] { "класическа музика", "оркестър", "концерт" });
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
//                if (text.Contains("волейбол")) tags.Add("волейбол");
//                break;

//            case EventCategory.Theatre:
//                if (text.Contains("драма")) tags.Add("драма");
//                if (text.Contains("комедия")) tags.Add("комедия");
//                if (text.Contains("мюзикъл")) tags.Add("мюзикъл");
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

//    private TaggingResult GenerateEnhancedFallbackTags(string eventName, string description, string? location)
//    {
//        var category = ClassifyWithKeywords(eventName, description);
//        var tags = ExtractTagsWithKeywords(eventName, description, location, category);

//        // Add music genres if it's a music event
//        if (category == EventCategory.Music)
//        {
//            var musicGenres = ExtractMusicGenresWithKeywords(eventName, description);
//            tags.AddRange(musicGenres);
//            tags = tags.Distinct().Take(6).ToList();
//        }

//        return new TaggingResult
//        {
//            SuggestedCategory = category,
//            SuggestedTags = tags,
//            Confidence = tags.ToDictionary(tag => tag, _ => 0.8)
//        };
//    }
//}

//// Response models for Groq
//public class GroqResponse
//{
//    [JsonPropertyName("choices")]
//    public GroqChoice[]? Choices { get; set; }
//}

//public class GroqChoice
//{
//    [JsonPropertyName("message")]
//    public GroqMessage? Message { get; set; }
//}

//public class GroqMessage
//{
//    [JsonPropertyName("content")]
//    public string? Content { get; set; }
//}

//public class PreciseAiTaggingResponse
//{
//    [JsonPropertyName("category")]
//    public int Category { get; set; }

//    [JsonPropertyName("subcategory")]
//    public string? Subcategory { get; set; }

//    [JsonPropertyName("tags")]
//    public List<string>? Tags { get; set; }
//}


//================
using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Events.Crawler.Services.Implementations;

public class GroqTaggingService : IAiTaggingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<GroqTaggingService> _logger;

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
        try
        {
            var prompt = $@"Analyze this Bulgarian event and classify it. Return ONLY the number (1-10):

Event: {eventName}
Description: {description}

Categories:
1=Music (концерти, музикални изпълнения, bands, DJ sets, festivals with music)
2=Art (изложби, галерии, visual arts, художествени инсталации)
3=Business (networking, бизнес събития, предприемачество, стартъп събития)
4=Sports (спортни събития, мачове, турнири, състезания)
5=Theatre (театрални постановки, драма, комедия, мюзикъли)
6=Cinema (кино прожекции, филми, премиери, документални)
7=Festivals (многодневни фестивали, културни празници, тематични събития)
8=Exhibitions (изложби, музеи, исторически експозиции)
9=Conferences (конференции, семинари, лекции, научни форуми)
10=Workshops (работилници, курсове, обучения, майсторски класове)

Detailed Examples:
- ""Slayer концерт в Зала 1"" = 1 (Music - metal concert)
- ""Iron Maiden live in Sofia"" = 1 (Music - rock concert)
- ""Джаз вечер с John Coltrane Tribute"" = 1 (Music - jazz performance)
- ""Изложба на Пикасо в НХГ"" = 2 (Art - painting exhibition)
- ""StartUp Bulgaria 2024"" = 3 (Business - startup event)
- ""Левски - ЦСКА дерби"" = 4 (Sports - football match)
- ""Хамлет в Народния театър"" = 5 (Theatre - drama performance)
- ""Премиера на Дюн 2"" = 6 (Cinema - movie premiere)
- ""Панаир на книгата София"" = 7 (Festival - cultural festival)
- ""Изложба Тракийско злато"" = 8 (Exhibition - historical exhibition)
- ""TEDx Sofia 2024"" = 9 (Conference - technology conference)
- ""UX/UI Design Workshop"" = 10 (Workshop - design training)

If you cannot determine the category with confidence, return 0.

Return only the number (0-10):";

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                model = "llama-3.1-8b-instant",
                max_tokens = 5,
                temperature = 0.1
            };

            var response = await _http.PostAsJsonAsync("openai/v1/chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
                var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

                if (int.TryParse(categoryText, out var categoryId))
                {
                    if (categoryId == 0)
                    {
                        _logger.LogWarning("AI could not classify '{EventName}' with confidence", eventName);
                        return null;
                    }

                    if (Enum.IsDefined(typeof(EventCategory), categoryId))
                    {
                        _logger.LogInformation("AI classified '{EventName}' as {Category}", eventName, (EventCategory)categoryId);
                        return (EventCategory)categoryId;
                    }
                }

                _logger.LogWarning("AI returned invalid response: '{Response}' for event '{EventName}'", categoryText, eventName);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("AI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
            }

            return TryClassifyWithKeywords(eventName, description);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error with AI classification for '{EventName}'", eventName);
            return TryClassifyWithKeywords(eventName, description);
        }
    }

    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
    {
        try
        {
            var category = await ClassifyEventAsync(eventName, description);
            var tags = ExtractTagsWithKeywords(eventName, description, location, category);
            var musicGenres = category == EventCategory.Music
                ? ExtractMusicGenresWithKeywords(eventName, description)
                : Enumerable.Empty<string>();

            var allTags = tags.Concat(musicGenres).Distinct().Take(6).ToList();

            _logger.LogInformation("Generated comprehensive tags for '{EventName}': Category={Category}, Tags=[{Tags}]",
                eventName, category?.ToString() ?? "None", string.Join(", ", allTags));

            return new TaggingResult
            {
                SuggestedCategory = category,
                SuggestedTags = allTags,
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
        return ExtractMusicGenresWithKeywords(eventName, description);
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
        var category = TryClassifyWithKeywords(eventName, description); // Използва TryClassifyWithKeywords
        var tags = ExtractTagsWithKeywords(eventName, description, location, category);

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
            SuggestedTags = tags,
            Confidence = tags.ToDictionary(tag => tag, _ => 0.8)
        };
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
using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class ClaudeProcessingService : IAiTaggingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly ILogger<ClaudeProcessingService> _logger;
    private static readonly SemaphoreSlim _rateLimiter = new(1, 1); // Reduced Max 1 concurrent request for safety
    private static DateTime _lastRequest = DateTime.MinValue;
    private static readonly object _lockObject = new();
    private static int _consecutiveFailures = 0; // Track consecutive failures

    public ClaudeProcessingService(HttpClient http, IConfiguration config, ILogger<ClaudeProcessingService> logger)
    {
        _http = http;
        _apiKey = config["Claude:ApiKey"] ?? throw new InvalidOperationException("Claude API key not configured");
        _logger = logger;

        // Anthropic Claude API endpoint
        _http.BaseAddress = new Uri("https://api.anthropic.com/v1/");
        _http.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
    }

    public async Task<EventCategory?> ClassifyEventAsync(string eventName, string description)
    {
        var result = await ProcessEventComprehensivelyAsync(eventName, description);
        return result.SuggestedCategory;
    }

    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
    {
        return await ProcessEventComprehensivelyAsync(eventName, description, location);
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

            var prompt = $@"You are an expert in cultural life in Sofia, Bulgaria. Your task is to categorize events based on your knowledge of the local cultural scene.

Below is the list of available categories and subcategories. Each subcategory includes example events in parentheses to help you choose the most appropriate one:

1=Music → Pop(Concerts of DARA, Mihaela Marinova), Rock(Midalidare Rock, B.T.R. concerts), Hip-Hop / Rap(100 Kila, Krisko, Fyre shows), Jazz(A to Jazz Festival, Plovdiv Jazz Fest), Blues(Sofia Blues Meeting), Classical(Sofia Philharmonic, Opera Open Plovdiv), Folk(Pirin Folk, traditional folklore events), Traditional Bulgarian(National Folklore Festivals), EDM(YALTA Club, Solar Events), Techno(Metropolis Events), House(EXE Club House Nights), Drum & Bass(HMSU events), Trance(Solar Trance Nights), Reggae(One Love Tour), R&B(Soul Sundays at Studio 5), Metal(Hills of Rock, Varna Mega Rock), Indie(Sofia Indie Nights), Acoustic(Acoustic Sessions Sofia), Alternative(Alarma Punk Jazz), Punk(Punk gigs at Mixtape 5), Soul(Smaller club concerts), Chillout(—), Experimental(—), Choir(Sofia Boys Choir), World Music(Ethno Jazz Fest), Other
2=Art → Painting(National Art Gallery exhibitions), Sculpture(—), Photography(PhotoSynthesis exhibitions), Digital Art(DA Fest), Street Art(Urban Creatures), Graffiti(Sofia Graffiti Tour), Illustration(—), Performance Art(Water Tower Art Fest), Installation Art(Contemporary gallery installations), Contemporary Art(Structura Gallery), Visual Arts(Sofia Art Week), Mixed Media(—), Conceptual Art(—), Other
3=Business → Networking Events(Founders Live), Startups(Startup Conference Bulgaria), Entrepreneurship(CEO Angels Club), Marketing(Digital4Sofia), Sales(—), Leadership(Leadership Talks), Finance(Investor Finance Forum), Real Estate(Expo Real Bulgaria), Investment(Money Motion), E-commerce(eCommerce Academy), Innovation(Innovation Explorer), Technology(Webit), HR & Management(HR Industry Expo), Business Strategy(—), Product Development(—), Other
4=Sports → Football(CSKA–Levski Derby), Basketball(NBL Games), Volleyball(National League), Tennis(Sofia Open), Athletics(—), Swimming(—), Running(Sofia Marathon), Cycling(Tour of Bulgaria), Boxing(MaxFight), MMA(Bulgarian Fighting Championship), Wrestling(—), Weightlifting(—), CrossFit(CrossFit Bulgarian Throwdowns), Yoga(Yoga in the Park), Fitness(FitFest), Hiking(Vitosha hikes), Climbing(Climb.bg events), Skiing(Bansko Ski Cup), Snowboarding(Pamporovo Freestyle), Motocross(Bulgarian Motocross Championship), eSports(A1 Gaming League), Table Tennis(—), Badminton(—), Golf(Pirin Golf Tournaments), Dance Sport(Dance Sport Championships), Other
5=Theatre → Drama(National Theatre performances), Comedy(Theatre Sofia), Musical Theatre(Musical productions at Opera Houses), Tragedy(—), Experimental Theatre(ACT Festival), Puppet Theatre(Puppet Theatre Sofia), Improvisation(HaHaHa Impro Theatre), Street Theatre(Street performances during festivals), Monodrama(One-man shows), Children's Theatre(Theatre “Pan”), Stand-up Comedy(Comedy Club Sofia, Inside Joke), Other
6=Cinema → Feature Films(SIFF), Short Films(In The Palace Film Fest), Documentaries(Master of Art Festival), Animation(Golden Kuker Sofia), Independent Cinema(Screenings at Dom na Kinoto), Bulgarian Cinema(Premiers at Odeon, Lumière Lidl), International Cinema(—), Film Premieres(Weekly premieres in cinemas), Student Films(NATFIZ screenings), Film Festivals(SIFF), Other
7=Festivals → Music Festivals(Hills of Rock, A to Jazz), Film Festivals(SIFF), Art Festivals(KvARTal Festival), Food & Wine Festivals(DiVino Taste), Cultural Festivals(Gabrovo Carnival), Folklore Festivals(Koprivshtitsa Fest), Street Festivals(Kapana Fest), Summer Festivals(Various coastal festivals), Light Festivals(LUNAR), Craft Beer Festivals(Sofia Brew Fest), Eco Festivals(Uzana Polyana Fest), Dance Festivals(Salsa Fest Bulgaria), Tech Festivals(Webit Expo), Other
8=Exhibitions → Art Exhibitions(National Gallery), Photography Exhibitions(PhotoSynthesis), Historical Exhibitions(National Museum), Science Exhibitions(Inter Expo Center), Technology Exhibitions(Tech Expo Sofia), Automotive Exhibitions(Sofia Motor Show), Design Exhibitions(Design Week), Cultural Heritage Exhibitions(Regional museum exhibitions), Educational Exhibitions(Book Fair Sofia), Craft Exhibitions(Handmade Fest), Other
9=Conferences → Tech Conferences(DEV.BG All In One, HackConf), Business Conferences(Webit), Startup Conferences(StartUP Conference), Academic Conferences(Sofia Science Festival), Marketing Conferences(DigitalK), Science Conferences(—), Health & Medicine Conferences(Medical Expo), AI & Innovation Conferences(AI Bulgaria Summit), IT Security Conferences(CyberSec Conferences), Environmental Conferences(Green Week), Other
10=Workshops → Art Workshops(Paint & Wine), Music Workshops(DJ Academy), Dance Workshops(Salsa workshops), Photography Workshops(PhotoSynthesis trainings), Cooking Workshops(Culinary Workshop Bulgaria), Craft Workshops(Handmade Workshops), Startup Workshops(Lean Startup Workshops), Personal Development Workshops(Public Speaking Bootcamps), Coding Workshops(CodeWeek), Language Workshops(English Bootcamps), Theatre Workshops(Acting masterclasses), Yoga Workshops(Yoga Retreats), Wellness Workshops(Breathwork sessions), Marketing Workshops(Social Media Workshops), Other
11=Undefined

Here is the event you need to categorize:
Event: {eventName}
Desc: {description}
Location: {location}
Use the event name, description, AND location (venues like Театър Сфумато, Кино Влайкова often signal the correct category) to determine the best category, subcategory, and tags.

Your task is to:
1. Select ONE category and ONE subcategory from the provided list that best fits this event
2. Add between 0 and 3 tags that help with event classification

Important rules for TAGS:
- Tags MUST be in English only and must provide additional useful classification information beyond what the category/subcategory already convey
- Never repeat or closely match the event name, venue name, category, or subcategory
- Never output city or venue names (e.g., Sofia, Teatar Sfoumato, Kino Vlaikova, NDK, club names); the system handles venue tagging separately and all events are already in Sofia
- Never use the words 'None' or 'Free'; if no tags apply, leave the tag section empty (e.g., 5|Drama|)
- Prefer high-value descriptors such as: Kids, Family, Educational, Networking, Legendary, International, Outdoor, Club, Evening, Morning, Weekend, Workshop, PetFriendly
- Tags should be general descriptors that help users filter and discover events
- Maximum 3 tags; only emit a tag if the event information clearly supports it

Before providing your answer, use the scratchpad to think through:
- What type of event this is based on the name, description, and location
- Which category and subcategory best fit
- What additional characteristics might be useful as tags
- Whether your proposed tags add value beyond the category/subcategory

CRITICAL: Return your answer ONLY in the format CATEGORY|SUBCATEGORY|tag1,tag2,tag3! If no suitable tags, keep the tag section empty
NO explanations! NO descriptions! ONLY the format!

Examples:
Slayer концерт → 1|Metal|thrash metal,concert,legendary
Пикасо изложба → 8|ArtExhibitions|art,painting
София филм фест → 7|FilmFestivals|films,festival
Пиеса за възрастни → 5|Drama|adult
Детски уикенд в парка → 1|Pop|kids,family,outdoors
Бизнес закуска за предприемачи → 3|Networking Events|educational,morning
Клубна нощ с международен DJ → 1|Electronic|club,international,nightlife

Return:";

            var requestBody = new ClaudeRequest
            {
                Model = "claude-3-haiku-20240307",
                MaxTokens = 120, // Increased for more robust output
                Temperature = 0.1,
                Messages = new[]
                {
                    new ClaudeMessageRequest { Role = "user", Content = prompt }
                }
            };

            var response = await _http.PostAsJsonAsync("messages", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>();
                var responseText = result?.Content?.FirstOrDefault()?.Text?.Trim();

                if (!string.IsNullOrEmpty(responseText))
                {
                    _logger.LogInformation("Claude AI response for '{EventName}': {Response}", eventName, responseText);

                    // Strict validation: Check if response follows format
                    if (!IsValidResponseFormat(responseText))
                    {
                        _logger.LogWarning("Claude returned invalid format for '{EventName}': {Response} - using fallback", eventName, responseText);
                        _consecutiveFailures++; // Count as failure
                        return GenerateEnhancedFallbackTags(eventName, description, location);
                    }

                    var parts = responseText.Split('|');
                    if (parts.Length >= 2 && int.TryParse(parts[0], out var categoryId))
                    {
                        if (categoryId == 11)
                        {
                            _logger.LogWarning("Claude classified '{EventName}' as Undefined", eventName);
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

                            _logger.LogInformation("Claude classified '{EventName}' as {Category}|{SubCategory} with tags: {Tags}",
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

                _logger.LogWarning("Claude returned invalid response: '{Response}' for event '{EventName}'", responseText, eventName);
                _consecutiveFailures++;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Claude API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                _consecutiveFailures++;
            }

            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("rate_limit_exceeded") || ex.Message.Contains("TooManyRequests"))
        {
            _consecutiveFailures++;
            _logger.LogWarning("Claude rate limit exceeded (failure #{Failures}), using fallback classification for '{EventName}'",
                _consecutiveFailures, eventName);
            return GenerateEnhancedFallbackTags(eventName, description, location);
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            _logger.LogError(ex, "Error in Claude AI processing for '{EventName}' (failure #{Failures})", eventName, _consecutiveFailures);
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
        var pattern = @"^\d+\|[A-Za-z]*\|[a-zA-Zа-яА-Я\s,]*$";
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

    // TODO: Extend the curated list of emblematic venues that should receive dedicated tags. Extend as needed when new landmarks appear.
    private static readonly (string Keyword, string Tag)[] SignificantVenueTags = new[]
    {
        ("театър натфиз", "театър натфиз"),
        ("тба", "тба"),
        ("театър 199", "театър 199"),
        ("университетски театър нбу", "университетски театър нбу"),
        ("зала българия", "зала българия"),
        ("арена 888", "арена 888"),
        ("бнр", "бнр"),
        ("топлоцентрала", "топлоцентрала"),
        ("малката текила", "малката текила"),
        ("podlez galeriya", "podlez galeriya"),
        ("joy station", "joy station"),
        ("борисова градина", "борисова градина"),
        ("юнак", "юнак"),
        ("търговски център", "търговски център")
    };

    private static readonly (string Keyword, string Tag)[] GenericVenueTags = new[]
    {
        ("club", "club"),
        ("клуб", "club"),
        ("ндк", "ndk"),
        ("бар", "bar"),
        ("galeria", "gallery"),
        ("галерия", "gallery"),
        ("опера", "opera"),
        ("колодрум", "colodrum"),
        ("парк", "park"),
        ("читалище", "chitalishte"),
        ("зала", "hall")
    };

    private static string? GetGenericVenueTag(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        var lowered = text.ToLowerInvariant();
        foreach (var (keyword, tag) in GenericVenueTags)
        {
            if (lowered.Contains(keyword))
            {
                return tag;
            }
        }

        return null;
    }

    private string? NormalizeLocation(string? location)
    {
        if (string.IsNullOrWhiteSpace(location)) return null;

        var normalized = location.ToLower().Trim();

        foreach (var (keyword, tag) in SignificantVenueTags)
        {
            if (normalized.Contains(keyword))
            {
                return tag;
            }
        }

        return null;
    }

    private List<string> CleanAndValidateTags(List<string> tags)
    {
        var cleaned = new List<string>();

        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag)) continue;

            var cleanTag = tag.Trim().ToLower();

            if (cleanTag is "none" or "безплатно") continue;

            if (cleanTag.Length > 30 || cleanTag.Contains("№") || cleanTag.Contains("ул.")) continue;

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

    private List<string> ExtractTagsWithKeywords(string eventName, string description, string? location, EventCategory? category)
    {
        var tags = new List<string>();

        var venueTag = GetGenericVenueTag(eventName) ?? GetGenericVenueTag(location) ?? NormalizeLocation(location);

        if (!string.IsNullOrWhiteSpace(venueTag))
        {
            tags.Add(venueTag);
        }

        return CleanAndValidateTags(tags).Take(6).ToList();
    }
}

// Claude API request/response models
public class ClaudeRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("messages")]
    public ClaudeMessageRequest[] Messages { get; set; } = Array.Empty<ClaudeMessageRequest>();
}

public class ClaudeMessageRequest
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class ClaudeResponse
{
    [JsonPropertyName("content")]
    public List<ClaudeContent> Content { get; set; } = new();
}

public class ClaudeContent
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
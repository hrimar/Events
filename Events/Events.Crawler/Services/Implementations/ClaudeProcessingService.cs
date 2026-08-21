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
            return FallbackClassification(eventName, description, location);
        }

        await _rateLimiter.WaitAsync();
        try
        {
            await EnsureRateLimit();

            var prompt = $@"You are an expert in the cultural and social life of Sofia, Bulgaria. Your task is to categorize events based on your knowledge of the local scene.

Below is the list of available categories and subcategories. Each subcategory includes example events in parentheses to help you choose the most appropriate one.

Output the subcategory **key exactly as written before the parenthesis** (e.g. `ThemedParty`, `WineTasting`, `HipHop`) — do not translate it and do not add spaces.

---

## CATEGORIES

**1 = Music** → Pop(DARA, Mihaela Marinova), Rock(Midalidare Rock, B.T.R.), HipHop(100 Kila, Krisko), Rap(Fyre shows), Jazz(A to Jazz Festival), Blues(Sofia Blues Meeting), Classical(Sofia Philharmonic), Folk(Pirin Folk), TraditionalBulgarian(National Folklore Festivals), EDM(YALTA Club, Solar Events), Techno(Metropolis Events), House(EXE Club House Nights), DrumBass(HMSU events), Trance(Solar Trance Nights), Reggae(One Love Tour), RB(Soul Sundays at Studio 5), Metal(Hills of Rock), Indie(Sofia Indie Nights), Acoustic(Acoustic Sessions Sofia), Alternative(Alarma Punk Jazz), Punk(Punk gigs at Mixtape 5), Soul(smaller club concerts), Chillout(—), Experimental(—), Choir(Sofia Boys Choir), WorldMusic(Ethno Jazz Fest), Other

**2 = Art** → Painting(National Art Gallery), Sculpture(—), Photography(PhotoSynthesis), DigitalArt(DA Fest), StreetArt(Urban Creatures), Graffiti(Sofia Graffiti Tour), Illustration(—), PerformanceArt(Water Tower Art Fest), InstallationArt(gallery installations), ContemporaryArt(Structura Gallery), VisualArts(Sofia Art Week), MixedMedia(—), ConceptualArt(—), Other

**3 = Business** → NetworkingEvents(Founders Live), Startups(Startup Conference Bulgaria), Entrepreneurship(CEO Angels Club), Marketing(Digital4Sofia), Sales(—), Leadership(Leadership Talks), Finance(Investor Finance Forum), RealEstate(Expo Real Bulgaria), Investment(Money Motion), ECommerce(eCommerce Academy), Innovation(Innovation Explorer), Technology(Webit), HRManagement(HR Industry Expo), BusinessStrategy(—), ProductDevelopment(—), Other

**4 = Sports** → Football(CSKA–Levski), Basketball(NBL Games), Volleyball(National League), Tennis(Sofia Open), Athletics(—), Swimming(—), Running(Sofia Marathon), Cycling(Tour of Bulgaria), Boxing(MaxFight), MMA(Bulgarian Fighting Championship), Wrestling(—), Weightlifting(—), CrossFit(Bulgarian Throwdowns), Yoga(Yoga in the Park), Fitness(FitFest), Hiking(Vitosha hikes), Climbing(Climb.bg), Skiing(Bansko Ski Cup), Snowboarding(Pamporovo Freestyle), Motocross(Bulgarian Championship), ESports(A1 Gaming League), TableTennis(—), Badminton(—), Golf(Pirin Golf), DanceSport(Dance Sport Championships), Other

**5 = Theatre** → Drama(National Theatre), Comedy(Theatre Sofia), MusicalTheatre(Opera House musicals), Tragedy(—), ExperimentalTheatre(ACT Festival), PuppetTheatre(Puppet Theatre Sofia), Improvisation(HaHaHa Impro), StreetTheatre(street performances), Monodrama(one-man shows), ChildrensTheatre(Theatre Pan), StandUpComedy(Comedy Club Sofia, Inside Joke), Other

**6 = Cinema** → FeatureFilms(SIFF), ShortFilms(In The Palace), Documentaries(Master of Art), Animation(Golden Kuker), IndependentCinema(Dom na Kinoto), BulgarianCinema(Odeon premieres), InternationalCinema(—), FilmPremieres(weekly premieres), StudentFilms(NATFIZ), FilmFestivals(SIFF), Other

**7 = Festivals** → MusicFestivals(Hills of Rock), FilmFestivals(SIFF), ArtFestivals(KvARTal), FoodWineFestivals(DiVino Taste), CulturalFestivals(Gabrovo Carnival), FolkloreFestivals(Koprivshtitsa), StreetFestivals(Kapana Fest), SummerFestivals(coastal festivals), LightFestivals(LUNAR), CraftBeerFestivals(Sofia Brew Fest), EcoFestivals(Uzana Polyana), DanceFestivals(Salsa Fest), TechFestivals(Webit Expo), Other

**8 = Exhibitions** → ArtExhibitions(National Gallery), PhotographyExhibitions(PhotoSynthesis), HistoricalExhibitions(National Museum), ScienceExhibitions(Inter Expo Center), TechnologyExhibitions(Tech Expo), AutomotiveExhibitions(Sofia Motor Show), DesignExhibitions(Design Week), CulturalHeritageExhibitions(regional museums), EducationalExhibitions(Book Fair), CraftExhibitions(Handmade Fest), Other

**9 = Conferences** → TechConferences(DEV.BG, HackConf), BusinessConferences(Webit), StartupConferences(StartUP Conference), AcademicConferences(Sofia Science Festival), MarketingConferences(DigitalK), ScienceConferences(—), HealthMedicineConferences(Medical Expo), AIInnovationConferences(AI Bulgaria Summit), ITSecurityConferences(CyberSec), EnvironmentalConferences(Green Week), Other

**10 = Workshops** → ArtWorkshops(Paint & Wine), MusicWorkshops(DJ Academy), DanceWorkshops(salsa lessons), PhotographyWorkshops(PhotoSynthesis trainings), CookingWorkshops(Culinary Workshop Bulgaria), CraftWorkshops(Handmade Workshops), StartupWorkshops(Lean Startup), PersonalDevelopmentWorkshops(Public Speaking Bootcamps), CodingWorkshops(CodeWeek), LanguageWorkshops(English Bootcamps), TheatreWorkshops(acting masterclasses), YogaWorkshops(Yoga Retreats), WellnessWorkshops(breathwork sessions), MarketingWorkshops(Social Media Workshops), Other

**11 = Entertainment** → Party(club parties with no announced artist), ThemedParty(Gatsby Night, Watermelon Party, Retro Party, 90s Night), ClubNight(club nights / DJ sets with no headline artist), Quiz(pub quiz, trivia night), Karaoke(karaoke nights), BoardGames(board game nights, D&D), EscapeRoom(escape rooms, quests), SocialDance(salsa/bachata/swing party where people dance), Other

**12 = FoodDrink** → WineTasting(wine tastings), BeerTasting(craft beer evenings), SpiritsTasting(Sofia Finest White Spirits, whisky evenings), CulinaryDinner(chef dinners, special menus), Brunch(brunch events), StreetFood(street food events), CoffeeTea(coffee cuppings, tea ceremonies), Other

**13 = Markets** → CraftMarket(handmade markets), FarmersMarket(farmers markets), FleaMarket(flea and vintage markets), ChristmasMarket(Christmas markets), DesignMarket(design markets), BookMarket(book fairs, Aleya na knigata), Other

**14 = Undefined** (use only when nothing above fits)

---

## HOW TO CHOOSE THE CATEGORY

Ask yourself: **why would someone attend this event?**

| Reason to attend | Category |
|---|---|
| To watch or listen to a performance | Music, Theatre, Cinema |
| To look at works or objects on display | Exhibitions, Art |
| To learn or to do something hands-on | Workshops, Conferences, Business |
| To compete, train or move | Sports |
| To eat, drink or taste | FoodDrink |
| To browse and buy goods from vendors | Markets |
| To socialize and be entertained | Entertainment |
| A multi-day or multi-act programme | Festivals |

---

## DISAMBIGUATION RULES

These cases are frequently misclassified. Follow them strictly.

1. **Parties.** If the title or description names a performing artist or DJ → **Music** + the matching genre subcategory. If the draw is a theme or format rather than a named artist (Gatsby Night, Watermelon Party, Retro Party, 90s Night) → **Entertainment / ThemedParty**.

2. **Quiz and trivia** → **Entertainment / Quiz**. Never Workshops.

3. **Dance events** — three different cases:
   - People come to dance socially (salsa, bachata, swing party) → **Entertainment / SocialDance**
   - People are being taught → **Workshops / DanceWorkshops**
   - A competition is being judged → **Sports / DanceSport**

4. **Food events** — two cases:
   - Participants cook → **Workshops / CookingWorkshops**
   - Participants eat, drink or taste → **FoodDrink**

5. **Stand-up comedy** → **Theatre / StandUpComedy**, not Entertainment.

6. **Children's events.** Choose the category by the art form or activity, then add the tag `For kids`. A puppet show is **Theatre / PuppetTheatre** + `For kids`. A children's art workshop is **Workshops / ArtWorkshops** + `For kids`. There is no separate children's category.

7. **Walking and hiking.** A mountain or nature hike → **Sports / Hiking**. A guided sightseeing walk around the city has no dedicated category yet — use the closest fitting category, or Undefined.

8. **Multi-day festivals** → **Festivals**, even when the content is music or film.

9. **Markets vs Exhibitions.** If vendors are selling goods → **Markets**. If works are displayed to be viewed → **Exhibitions**.

---

## INPUT

Here is the event you need to categorize:
Event: {eventName}
Desc: {description}
Location: {location}

The location (venue) is a strong signal (Театър Сфумато → Theatre; Кино Влайкова → Cinema; Bar Singles / клуб → Music or Entertainment; Зала България → Music/Classical).

---

## YOUR TASK

1. Select **ONE category** and **ONE subcategory** from the list above.
2. Add **between 0 and 3 tags** from the approved master tag list only.

### Approved master tag list

`Adults only`, `Beginners`, `Bulgarian culture`, `Contemporary`, `English-friendly`, `Family-friendly`, `For kids`, `Hands-on`, `Holiday special`, `Indoor`, `Interactive`, `International guests`, `Live`, `Local artists`, `Networking`, `Outdoor`, `Professionals`, `Seasonal`, `Students`, `Team-building`, `Traditional`

### Rules for tags

- Tags MUST be in English and MUST come from the approved list only. Never invent new tags.
- **`Live` means a live human performance only** — a band, orchestra, actors, stand-up comedian, live singer. Do **NOT** use `Live` for DJ sets, club nights, film screenings, quizzes, markets, tastings or exhibitions.
- Tags must add information **beyond** what the category and subcategory already say.
- Do NOT repeat or paraphrase the event name, location (venue) name, category or subcategory.
- Do NOT output city or location (venue) names (Sofia, NDK, club names) — the system handles location (venue) data separately and all events are in Sofia.
- Maximum 3 tags. Returning 0, 1 or 2 tags is better than adding weak or redundant ones.

### Tag selection algorithm

1. Determine the category and subcategory first.
2. Then check whether a tag adds real value beyond that choice.
3. Prefer tags across different dimensions:
   - audience — `Family-friendly`, `For kids`, `Beginners`, `Professionals`, `Students`, `Adults only`
   - format — `Live`, `Outdoor`, `Indoor`, `Networking`, `Hands-on`, `Interactive`, `Team-building`
   - cultural relevance — `Bulgarian culture`, `Local artists`, `International guests`, `Traditional`, `Contemporary`, `English-friendly`
   - timing — `Seasonal`, `Holiday special`
4. If two tags express the same idea, keep only one.
5. Skip any tag that is already obvious from the category, subcategory or title.

---

## OUTPUT

Think through the classification internally. Do **not** write out your reasoning.

Return your answer ONLY in this format:

```
CATEGORY|SUBCATEGORY|tag1,tag2,tag3
```

If no suitable tags apply, leave the tag section empty:

```
CATEGORY|SUBCATEGORY|
```

NO scratchpad. NO explanations. NO descriptions. ONLY the format.

---

## WORKED EXAMPLES

| Event | Correct output |
|---|---|
| QUIZ ВЕЧЕР в Бар ""Тънка Червена Линия"" | `Entertainment\|Quiz\|Interactive,Team-building` |
| Нощта на Гетсби @ Малката Текила | `Entertainment\|ThemedParty\|Adults only` |
| Swinging Summer Vol.2 \| Swing Dance Party | `Entertainment\|SocialDance\|Beginners` |
| ATB - SPICE Music Festival Pre-party | `Music\|Trance\|International guests` |
| FRIDAY AFTERNOON PARTY WITH MITKO PAVLOV | `Music\|House\|Local artists` |
| Sofia Finest White Spirits | `FoodDrink\|SpiritsTasting\|Adults only` |
| SCORPIONS @ Арена 8888 | `Music\|Rock\|Live,International guests` |
| Кино под звездите: България Amore Mio | `Cinema\|BulgarianCinema\|Outdoor,Bulgarian culture` |
| Дядовата ръкавичка (куклен спектакъл) | `Theatre\|PuppetTheatre\|For kids` |
| Аз рисувам като Пийт Мондриан — ателие за деца | `Workshops\|ArtWorkshops\|For kids,Hands-on` |";

            var requestBody = new ClaudeRequest
            {
                Model = "claude-haiku-4-5-20251001", // "claude-3-haiku-20240307" was deprecated
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
                        return FallbackClassification(eventName, description, location);
                    }

                    var parts = responseText.Split('|');
                    // Claude sometimes echoes the category name: "1=Music" — strip the name part
                    var categoryPart = parts[0].Split('=')[0].Trim();
                    if (parts.Length >= 2 && int.TryParse(categoryPart, out var categoryId))
                    {
                        if (categoryId == (int)EventCategory.Undefined)
                        {
                            _logger.LogWarning("Claude classified '{EventName}' as Undefined", eventName);
                            _consecutiveFailures = Math.Max(0, _consecutiveFailures - 1); // Partial success
                            return FallbackClassification(eventName, description, location);
                        }

                        if (Enum.IsDefined(typeof(EventCategory), categoryId))
                        {
                            var category = (EventCategory)categoryId;
                            var subcategory = parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]) ? parts[1].Trim() : "Other";

                            var aiTags = parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2])
                                ? parts[2].Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList()
                                : new List<string>();

                            _logger.LogInformation("Claude classified '{EventName}' as {Category}|{SubCategory} with tags: {Tags}",
                                eventName, category, subcategory, string.Join(", ", aiTags));

                            // Reset failure counter
                            _consecutiveFailures = 0;

                            return new TaggingResult
                            {
                                SuggestedCategory = category,
                                SuggestedSubCategory = subcategory,
                                SuggestedTags = aiTags,
                                Confidence = aiTags.ToDictionary(tag => tag, _ => 0.85)
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

            return FallbackClassification(eventName, description, location);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("rate_limit_exceeded") || ex.Message.Contains("TooManyRequests"))
        {
            _consecutiveFailures++;
            _logger.LogWarning("Claude rate limit exceeded (failure #{Failures}), using fallback classification for '{EventName}'", _consecutiveFailures, eventName);
            return FallbackClassification(eventName, description, location);
        }
        catch (Exception ex)
        {
            _consecutiveFailures++;
            _logger.LogError(ex, "Error in Claude AI processing for '{EventName}' (failure #{Failures})", eventName, _consecutiveFailures);
            return FallbackClassification(eventName, description, location);
        }
        finally
        {
            _rateLimiter.Release();
        }
    }

    // Strict format validation
    private static bool IsValidResponseFormat(string response)
    {
        // Must match: NUMBER|SUBCATEGORY|tags
        // Allow apostrophes (Children's), ampersands (R&B), = (when Claude echoes category name)
        var pattern = @"^\d+[^|]*\|[^|]*\|[a-zA-Z\s,\-'&]*$";
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

        if (new[] { "куиз", "quiz", "тривия", "trivia", "парти", "party", "караоке", "karaoke", "настолни игри", "board game", "ескейп рум", "escape room" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Entertainment", eventName);
            return EventCategory.Entertainment;
        }

        if (new[] { "дегустация", "tasting", "бранч", "brunch", "стрийт фууд", "street food", "кулинарна вечеря" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as FoodDrink", eventName);
            return EventCategory.FoodDrink;
        }

        if (new[] { "пазар", "базар", "market" }.Any(keyword => text.Contains(keyword)))
        {
            _logger.LogInformation("Keyword classified '{EventName}' as Markets", eventName);
            return EventCategory.Markets;
        }

        _logger.LogWarning("Could not classify '{EventName}' - needs manual categorization", eventName);
        return null;
    }

    private TaggingResult FallbackClassification(string eventName, string description, string? location)
    {
        var category = TryClassifyWithKeywords(eventName, description);

        string? suggestedSubCategory = null;
        if (category.HasValue)
        {
            suggestedSubCategory = ExtractSubCategory(eventName, description, category.Value);
        }

        return new TaggingResult
        {
            SuggestedCategory = category,
            SuggestedSubCategory = suggestedSubCategory,
            SuggestedTags = new List<string>(),
            Confidence = new Dictionary<string, double>()
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
            EventCategory.Entertainment => ExtractEntertainmentSubCategory(text),
            EventCategory.FoodDrink => ExtractFoodDrinkSubCategory(text),
            EventCategory.Markets => ExtractMarketsSubCategory(text),
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

    private string? ExtractEntertainmentSubCategory(string text)
    {
        if (new[] { "куиз", "quiz", "тривия", "trivia" }.Any(keyword => text.Contains(keyword)))
            return "Quiz";

        if (new[] { "караоке", "karaoke" }.Any(keyword => text.Contains(keyword)))
            return "Karaoke";

        if (new[] { "настолни игри", "board game" }.Any(keyword => text.Contains(keyword)))
            return "BoardGames";

        if (new[] { "ескейп рум", "escape room" }.Any(keyword => text.Contains(keyword)))
            return "EscapeRoom";

        if (new[] { "танци", "salsa", "bachata", "swing" }.Any(keyword => text.Contains(keyword)))
            return "SocialDance";

        if (new[] { "тематично парти", "themed party" }.Any(keyword => text.Contains(keyword)))
            return "ThemedParty";

        if (new[] { "парти", "party" }.Any(keyword => text.Contains(keyword)))
            return "Party";

        return "Other";
    }

    private string? ExtractFoodDrinkSubCategory(string text)
    {
        if (new[] { "вино", "wine" }.Any(keyword => text.Contains(keyword)))
            return "WineTasting";

        if (new[] { "бира", "beer" }.Any(keyword => text.Contains(keyword)))
            return "BeerTasting";

        if (new[] { "спиртни напитки", "spirits", "уиски", "whisky" }.Any(keyword => text.Contains(keyword)))
            return "SpiritsTasting";

        if (new[] { "бранч", "brunch" }.Any(keyword => text.Contains(keyword)))
            return "Brunch";

        if (new[] { "стрийт фууд", "street food" }.Any(keyword => text.Contains(keyword)))
            return "StreetFood";

        if (new[] { "кафе", "coffee", "чай", "tea" }.Any(keyword => text.Contains(keyword)))
            return "CoffeeTea";

        if (new[] { "кулинарна вечеря", "culinary dinner" }.Any(keyword => text.Contains(keyword)))
            return "CulinaryDinner";

        return "Other";
    }

    private string? ExtractMarketsSubCategory(string text)
    {
        if (new[] { "фермерски пазар", "farmers market" }.Any(keyword => text.Contains(keyword)))
            return "FarmersMarket";

        if (new[] { "битпазар", "flea market" }.Any(keyword => text.Contains(keyword)))
            return "FleaMarket";

        if (new[] { "коледен пазар", "christmas market" }.Any(keyword => text.Contains(keyword)))
            return "ChristmasMarket";

        if (new[] { "дизайн пазар", "design market" }.Any(keyword => text.Contains(keyword)))
            return "DesignMarket";

        if (new[] { "книжен пазар", "book market", "book fair" }.Any(keyword => text.Contains(keyword)))
            return "BookMarket";

        if (new[] { "хендмейд", "craft market", "занаяти" }.Any(keyword => text.Contains(keyword)))
            return "CraftMarket";

        return "Other";
    }

    private async Task EnsureRateLimit()
    {
        TimeSpan delay;

        lock (_lockObject)
        {
            var timeSinceLastRequest = DateTime.UtcNow - _lastRequest;

            var requiredDelay = _consecutiveFailures > 0
                ? TimeSpan.FromMilliseconds(2000 + (_consecutiveFailures * 1000))
                : TimeSpan.FromMilliseconds(1000);

            delay = timeSinceLastRequest < requiredDelay
                ? requiredDelay - timeSinceLastRequest
                : TimeSpan.Zero;

            _lastRequest = DateTime.UtcNow + delay;
        }

        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay);
        }
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
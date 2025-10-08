// Payed versions only

//using Events.Crawler.Models;
//using Events.Crawler.Services.Interfaces;
//using Events.Models.Enums;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Events.Crawler.Services.Implementations;

//public class ClaudeApiClient : IAiTaggingService
//{
//    private readonly HttpClient _http;
//    private readonly string _apiKey;
//    private readonly ILogger<ClaudeApiClient> _logger;

//    public ClaudeApiClient(HttpClient http, IConfiguration config, ILogger<ClaudeApiClient> logger)
//    {
//        _http = http;
//        _apiKey = config["Claude:ApiKey"] ?? throw new InvalidOperationException("Claude API key not configured");
//        _logger = logger;

//        _http.BaseAddress = new Uri("https://api.anthropic.com/");
//        _http.DefaultRequestHeaders.Add("x-api-key", _apiKey);
//        _http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
//    }

//    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $"""
//            Анализирай следното събитие и определи категорията му. Върни само числото на категорията.

//            Заглавие: {eventName}
//            Описание: {description}

//            Категории:
//            1 - Music (музика)
//            2 - Art (изкуство) 
//            3 - Business (бизнес)
//            4 - Sports (спорт)
//            5 - Theatre (театър)
//            6 - Cinema (кино)
//            7 - Festivals (фестивали)
//            8 - Exhibitions (изложби)
//            9 - Conferences (конференции)
//            10 - Workshops (работилници)

//            Върни само числото на най-подходящата категория:
//            """;

//            var request = new
//            {
//                model = "claude-3-haiku-20240307",
//                max_tokens = 10,
//                temperature = 0.1,
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                }
//            };

//            var response = await _http.PostAsJsonAsync("v1/messages", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>();
//            var categoryText = result?.Content?.FirstOrDefault()?.Text?.Trim();

//            if (int.TryParse(categoryText, out var categoryId) &&
//                Enum.IsDefined(typeof(EventCategory), categoryId))
//            {
//                return (EventCategory)categoryId;
//            }

//            _logger.LogWarning("Failed to parse category from Claude response: {Response}", categoryText);
//            return EventCategory.Exhibitions; // Default fallback
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error classifying event with Claude API");
//            return EventCategory.Exhibitions; // Default fallback
//        }
//    }

//    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
//    {
//        try
//        {
//            var prompt = $"""
//            Анализирай следното събитие и генерирай подходящи тагове на български език:

//            Заглавие: {eventName}
//            Описание: {description}
//            Местоположение: {location ?? "Неизвестно"}

//            Генерирай 3-7 тагове, които най-добре описват събитието. Таговете трябва да са:
//            - На български език
//            - Релевантни към съдържанието
//            - Полезни за търсене и филтриране
//            - Кратки (1-3 думи)

//            Върни таговете като JSON масив от стрингове:
//            """;

//            var request = new
//            {
//                model = "claude-3-haiku-20240307",
//                max_tokens = 200,
//                temperature = 0.3,
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                }
//            };

//            var response = await _http.PostAsJsonAsync("v1/messages", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>();
//            var tagsText = result?.Content?.FirstOrDefault()?.Text?.Trim();

//            if (!string.IsNullOrEmpty(tagsText))
//            {
//                try
//                {
//                    var tags = JsonSerializer.Deserialize<string[]>(tagsText);
//                    if (tags != null && tags.Length > 0)
//                    {
//                        return new TaggingResult
//                        {
//                            SuggestedTags = tags.ToList(),
//                            Confidence = tags.ToDictionary(tag => tag, _ => 0.8) // Default confidence
//                        };
//                    }
//                }
//                catch (JsonException ex)
//                {
//                    _logger.LogWarning(ex, "Failed to parse tags JSON from Claude: {Response}", tagsText);

//                    // Fallback: split by comma
//                    var fallbackTags = tagsText
//                        .Replace("[", "").Replace("]", "").Replace("\"", "")
//                        .Split(',')
//                        .Select(t => t.Trim())
//                        .Where(t => !string.IsNullOrEmpty(t))
//                        .Take(7)
//                        .ToList();

//                    if (fallbackTags.Any())
//                    {
//                        return new TaggingResult
//                        {
//                            SuggestedTags = fallbackTags,
//                            Confidence = fallbackTags.ToDictionary(tag => tag, _ => 0.6)
//                        };
//                    }
//                }
//            }

//            // Fallback to generic tags based on event name
//            return GenerateFallbackTags(eventName, description);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error generating tags with Claude API");
//            return GenerateFallbackTags(eventName, description);
//        }
//    }

//    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $"""
//            Анализирай следното музикално събитие и определи музикалните жанрове:

//            Заглавие: {eventName}
//            Описание: {description}

//            Музикални жанрове (върни до 3 най-точни):
//            рок, джаз, метъл, поп, фънк, пънк, опера, класическа, фолк, електронна, хип-хоп, блус, регей, кънтри

//            Върни само жанровете като JSON масив:
//            """;

//            var request = new
//            {
//                model = "claude-3-haiku-20240307",
//                max_tokens = 100,
//                temperature = 0.2,
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                }
//            };

//            var response = await _http.PostAsJsonAsync("v1/messages", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>();
//            var genresText = result?.Content?.FirstOrDefault()?.Text?.Trim();

//            if (!string.IsNullOrEmpty(genresText))
//            {
//                try
//                {
//                    var genres = JsonSerializer.Deserialize<string[]>(genresText);
//                    return genres ?? new[] { "музика" };
//                }
//                catch (JsonException)
//                {
//                    // Fallback parsing
//                    return genresText
//                        .Replace("[", "").Replace("]", "").Replace("\"", "")
//                        .Split(',')
//                        .Select(g => g.Trim())
//                        .Where(g => !string.IsNullOrEmpty(g))
//                        .Take(3);
//                }
//            }

//            return new[] { "музика" };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error extracting music genres with Claude API");
//            return new[] { "музика" };
//        }
//    }

//    private TaggingResult GenerateFallbackTags(string eventName, string description)
//    {
//        var fallbackTags = new List<string>();

//        // Add basic tags based on event name
//        if (eventName.ToLower().Contains("концерт"))
//            fallbackTags.Add("концерт");
//        if (eventName.ToLower().Contains("театър"))
//            fallbackTags.Add("театър");
//        if (eventName.ToLower().Contains("изложба"))
//            fallbackTags.Add("изложба");
//        if (eventName.ToLower().Contains("фестивал"))
//            fallbackTags.Add("фестивал");

//        // Add generic location tag
//        fallbackTags.Add("софия");

//        if (!fallbackTags.Any())
//            fallbackTags.Add("събитие");

//        return new TaggingResult
//        {
//            SuggestedTags = fallbackTags,
//            Confidence = fallbackTags.ToDictionary(tag => tag, _ => 0.5)
//        };
//    }
//}

//// Response models for Claude API
//public class ClaudeResponse
//{
//    [JsonPropertyName("content")]
//    public ClaudeContent[]? Content { get; set; }
//}

//public class ClaudeContent
//{
//    [JsonPropertyName("text")]
//    public string? Text { get; set; }
//}
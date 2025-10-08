//using Events.Crawler.Models;
//using Events.Crawler.Services.Interfaces;
//using Events.Models.Enums;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Events.Crawler.Services.Implementations;

//public class AzureOpenAiTaggingService : IAiTaggingService
//{
//    private readonly HttpClient _http;
//    private readonly string _apiKey;
//    private readonly string _endpoint;
//    private readonly ILogger<AzureOpenAiTaggingService> _logger;

//    public AzureOpenAiTaggingService(HttpClient http, IConfiguration config, ILogger<AzureOpenAiTaggingService> logger)
//    {
//        _http = http;
//        _apiKey = config["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("Azure OpenAI API key not configured");
//        _endpoint = config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Azure OpenAI endpoint not configured");
//        _logger = logger;
        
//        _http.DefaultRequestHeaders.Add("api-key", _apiKey);
//    }

//    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $@"Analyze this Bulgarian event and classify it. Return ONLY the number (1-10):

//Event: {eventName}
//Description: {description}

//Categories:
//1=Music, 2=Art, 3=Business, 4=Sports, 5=Theatre, 6=Cinema, 7=Festivals, 8=Exhibitions, 9=Conferences, 10=Workshops

//Answer with just the number:";

//            var requestBody = new
//            {
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                },
//                max_tokens = 5,
//                temperature = 0.1
//            };

//            var url = $"{_endpoint}/openai/deployments/gpt-35-turbo/chat/completions?api-version=2024-02-15-preview";
//            var response = await _http.PostAsJsonAsync(url, requestBody);
            
//            if (response.IsSuccessStatusCode)
//            {
//                var result = await response.Content.ReadFromJsonAsync<AzureOpenAiResponse>();
//                var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//                if (int.TryParse(categoryText, out var categoryId) && 
//                    Enum.IsDefined(typeof(EventCategory), categoryId))
//                {
//                    return (EventCategory)categoryId;
//                }
//            }

//            _logger.LogWarning("Failed to classify event, defaulting to Music");
//            return EventCategory.Music; // Sofia events are mostly music
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error with Azure OpenAI classification");
//            return EventCategory.Music;
//        }
//    }

//    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
//    {
//        try
//        {
//            var prompt = $@"Analyze this Sofia event and suggest category and tags in Bulgarian:

//Event: {eventName}
//Description: {description}
//Location: {location ?? "София"}

//Return JSON:
//{{
//    ""category"": number 1-10,
//    ""tags"": [""tag1"", ""tag2"", ""tag3""]
//}}

//Categories: 1=Music, 2=Art, 3=Business, 4=Sports, 5=Theatre, 6=Cinema, 7=Festivals, 8=Exhibitions, 9=Conferences, 10=Workshops";

//            var requestBody = new
//            {
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                },
//                max_tokens = 150,
//                temperature = 0.3
//            };

//            var url = $"{_endpoint}/openai/deployments/gpt-35-turbo/chat/completions?api-version=2024-02-15-preview";
//            var response = await _http.PostAsJsonAsync(url, requestBody);
            
//            if (response.IsSuccessStatusCode)
//            {
//                var result = await response.Content.ReadFromJsonAsync<AzureOpenAiResponse>();
//                var content = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//                if (!string.IsNullOrEmpty(content))
//                {
//                    try
//                    {
//                        var aiResult = JsonSerializer.Deserialize<AiTaggingResponse>(content, new JsonSerializerOptions
//                        {
//                            PropertyNameCaseInsensitive = true
//                        });

//                        if (aiResult != null)
//                        {
//                            var category = Enum.IsDefined(typeof(EventCategory), aiResult.Category) 
//                                ? (EventCategory)aiResult.Category 
//                                : EventCategory.Music;

//                            return new TaggingResult
//                            {
//                                SuggestedCategory = category,
//                                SuggestedTags = aiResult.Tags ?? new List<string> { "събитие" },
//                                Confidence = (aiResult.Tags ?? new List<string>()).ToDictionary(tag => tag, _ => 0.8)
//                            };
//                        }
//                    }
//                    catch (JsonException ex)
//                    {
//                        _logger.LogWarning(ex, "Failed to parse Azure OpenAI response");
//                    }
//                }
//            }

//            return GenerateFallbackTags(eventName, description);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error with Azure OpenAI tagging");
//            return GenerateFallbackTags(eventName, description);
//        }
//    }

//    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
//    {
//        // Similar implementation
//        return new[] { "музика" };
//    }

//    private TaggingResult GenerateFallbackTags(string eventName, string description)
//    {
//        var tags = new List<string> { "софия" };
        
//        var text = $"{eventName} {description}".ToLower();
//        if (text.Contains("концерт")) tags.Add("концерт");
//        if (text.Contains("музика")) tags.Add("музика");
        
//        return new TaggingResult
//        {
//            SuggestedCategory = EventCategory.Music,
//            SuggestedTags = tags,
//            Confidence = tags.ToDictionary(tag => tag, _ => 0.6)
//        };
//    }
//}

//public class AzureOpenAiResponse
//{
//    [JsonPropertyName("choices")]
//    public AzureOpenAiChoice[]? Choices { get; set; }
//}

//public class AzureOpenAiChoice
//{
//    [JsonPropertyName("message")]
//    public AzureOpenAiMessage? Message { get; set; }
//}

//public class AzureOpenAiMessage
//{
//    [JsonPropertyName("content")]
//    public string? Content { get; set; }
//}

//public class AiTaggingResponse
//{
//    [JsonPropertyName("category")]
//    public int Category { get; set; }
    
//    [JsonPropertyName("tags")]
//    public List<string>? Tags { get; set; }
//}
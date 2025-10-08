//// Too many request for the free version. Try with less requests

////using Events.Crawler.Models;
////using Events.Crawler.Services.Interfaces;
////using Events.Models.Enums;
////using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.Logging;
////using System.Net.Http.Json;
////using System.Text.Json;
////using System.Text.Json.Serialization;
////using System.Net;

////namespace Events.Crawler.Services.Implementations;

////public class OpenAiTaggingService : IAiTaggingService
////{
////    private readonly HttpClient _http;
////    private readonly string _apiKey;
////    private readonly ILogger<OpenAiTaggingService> _logger;
////    private readonly SemaphoreSlim _rateLimitSemaphore;
////    private DateTime _lastRequestTime = DateTime.MinValue;
////    private readonly TimeSpan _minimumRequestInterval = TimeSpan.FromMilliseconds(1000); // 1 second between requests

////    public OpenAiTaggingService(HttpClient http, IConfiguration config, ILogger<OpenAiTaggingService> logger)
////    {
////        _http = http;
////        _apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");
////        _logger = logger;
////        _rateLimitSemaphore = new SemaphoreSlim(1, 1); // Only 1 concurrent request

////        _http.BaseAddress = new Uri("https://api.openai.com/");
////        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
////        _http.Timeout = TimeSpan.FromSeconds(30); // 30 second timeout
////    }

////    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
////    {
////        try
////        {
////            var prompt = $@"Анализирай следното събитие и определи категорията му. Върни само числото на категорията.

////Заглавие: {eventName}
////Описание: {description}

////Категории:
////1 - Music (музика)
////2 - Art (изкуство) 
////3 - Business (бизнес)
////4 - Sports (спорт)
////5 - Theatre (театър)
////6 - Cinema (кино)
////7 - Festivals (фестивали)
////8 - Exhibitions (изложби)
////9 - Conferences (конференции)
////10 - Workshops (работилници)

////Върни само числото на най-подходящата категория (1-10):";

////            var request = new
////            {
////                model = "gpt-3.5-turbo",
////                messages = new[]
////                {
////                    new { role = "user", content = prompt }
////                },
////                max_tokens = 10,
////                temperature = 0.1
////            };

////            var response = await MakeRateLimitedRequestAsync("v1/chat/completions", request);
////            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
////            var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

////            if (int.TryParse(categoryText, out var categoryId) &&
////                Enum.IsDefined(typeof(EventCategory), categoryId))
////            {
////                return (EventCategory)categoryId;
////            }

////            _logger.LogWarning("Failed to parse category from OpenAI response: {Response}", categoryText);
////            return EventCategory.Exhibitions; // Default fallback
////        }
////        catch (Exception ex)
////        {
////            _logger.LogError(ex, "Error classifying event with OpenAI API");
////            return EventCategory.Exhibitions; // Default fallback
////        }
////    }

////    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
////    {
////        try
////        {
////            var prompt = $@"Анализирай следното събитие в София и предложи категория, подкатегория и тагове:

////Заглавие: {eventName}
////Описание: {description}
////Местоположение: {location ?? "София"}

////Върни отговора в JSON формат със следната структура:
////{{
////    ""category"": число от 1 до 10 (виж категориите по-долу),
////    ""subcategory"": конкретна подкатегория като стринг,
////    ""tags"": масив от 3-7 тагове на български език
////}}

////Категории:
////1=Music, 2=Art, 3=Business, 4=Sports, 5=Theatre, 6=Cinema, 7=Festivals, 8=Exhibitions, 9=Conferences, 10=Workshops

////Музикални подкатегории: рок, джаз, метъл, поп, фънк, пънк, опера, класическа, електронна, фолк, блус, кънтри, регей, хип-хоп, алтернативна

////Спортни подкатегории: футбол, баскетбол, тенис, волейбол, плуване, атлетика, бокс, борба, гимнастика, колоездене

////Таговете да включват: жанр/тип, настроение, възрастова група, време на деня, цена, стил и др.";

////            var request = new
////            {
////                model = "gpt-3.5-turbo",
////                messages = new[]
////                {
////                    new { role = "user", content = prompt }
////                },
////                max_tokens = 300,
////                temperature = 0.3
////            };

////            var response = await MakeRateLimitedRequestAsync("v1/chat/completions", request);
////            var content = await response.Content.ReadAsStringAsync();

////            if (!string.IsNullOrEmpty(content))
////            {
////                try
////                {
////                    var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
////                    var aiContent = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

////                    if (!string.IsNullOrEmpty(aiContent))
////                    {
////                        var aiResult = JsonSerializer.Deserialize<AiTaggingResponse>(aiContent, new JsonSerializerOptions
////                        {
////                            PropertyNameCaseInsensitive = true
////                        });

////                        if (aiResult != null)
////                        {
////                            var category = Enum.IsDefined(typeof(EventCategory), aiResult.Category)
////                                ? (EventCategory)aiResult.Category
////                                : EventCategory.Exhibitions;

////                            var tags = aiResult.Tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList() ?? new List<string>();

////                            // Add subcategory as a tag if provided
////                            if (!string.IsNullOrWhiteSpace(aiResult.Subcategory))
////                            {
////                                tags.Insert(0, aiResult.Subcategory);
////                            }

////                            return new TaggingResult
////                            {
////                                SuggestedCategory = category,
////                                SuggestedTags = tags,
////                                Confidence = tags.ToDictionary(tag => tag, _ => 0.8)
////                            };
////                        }
////                    }
////                }
////                catch (JsonException ex)
////                {
////                    _logger.LogWarning(ex, "Failed to parse OpenAI JSON response: {Content}", content);

////                    // Fallback parsing
////                    return GenerateFallbackTags(eventName, description);
////                }
////            }

////            return GenerateFallbackTags(eventName, description);
////        }
////        catch (Exception ex)
////        {
////            _logger.LogError(ex, "Error generating tags with OpenAI API");
////            return GenerateFallbackTags(eventName, description);
////        }
////    }

////    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
////    {
////        try
////        {
////            var prompt = $@"Анализирай следното музикално събитие и определи музикалните жанрове:

////Заглавие: {eventName}
////Описание: {description}

////Музикални жанрове: рок, джаз, метъл, поп, фънк, пънк, опера, класическа, електронна, фолк, блус, кънтри, регей, хип-хоп, алтернативна

////Върни до 3 най-точни жанра като JSON масив от стрингове:";

////            var request = new
////            {
////                model = "gpt-3.5-turbo",
////                messages = new[]
////                {
////                    new { role = "user", content = prompt }
////                },
////                max_tokens = 100,
////                temperature = 0.2
////            };

////            var response = await MakeRateLimitedRequestAsync("v1/chat/completions", request);
////            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
////            var content = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

////            if (!string.IsNullOrEmpty(content))
////            {
////                try
////                {
////                    var genres = JsonSerializer.Deserialize<string[]>(content);
////                    return genres ?? new[] { "музика" };
////                }
////                catch (JsonException)
////                {
////                    // Fallback parsing
////                    return content
////                        .Replace("[", "").Replace("]", "").Replace("\"", "")
////                        .Split(',')
////                        .Select(g => g.Trim())
////                        .Where(g => !string.IsNullOrEmpty(g))
////                        .Take(3);
////                }
////            }

////            return new[] { "музика" };
////        }
////        catch (Exception ex)
////        {
////            _logger.LogError(ex, "Error extracting music genres with OpenAI API");
////            return new[] { "музика" };
////        }
////    }

////    private async Task<HttpResponseMessage> MakeRateLimitedRequestAsync<T>(string endpoint, T requestBody)
////    {
////        await _rateLimitSemaphore.WaitAsync();

////        try
////        {
////            // Ensure minimum time between requests
////            var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
////            if (timeSinceLastRequest < _minimumRequestInterval)
////            {
////                var delay = _minimumRequestInterval - timeSinceLastRequest;
////                _logger.LogDebug("Rate limiting: waiting {Delay}ms before next request", delay.TotalMilliseconds);
////                await Task.Delay(delay);
////            }

////            _lastRequestTime = DateTime.UtcNow;

////            // Retry logic for rate limiting
////            for (int attempt = 1; attempt <= 3; attempt++)
////            {
////                try
////                {
////                    var response = await _http.PostAsJsonAsync(endpoint, requestBody);

////                    if (response.IsSuccessStatusCode)
////                    {
////                        return response;
////                    }

////                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
////                    {
////                        var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Exponential backoff
////                        _logger.LogWarning("Rate limited by OpenAI. Attempt {Attempt}. Waiting {Delay} seconds", attempt, retryAfter.TotalSeconds);

////                        await Task.Delay(retryAfter);
////                        continue;
////                    }

////                    response.EnsureSuccessStatusCode();
////                    return response;
////                }
////                catch (HttpRequestException ex) when (ex.Message.Contains("429") && attempt < 3)
////                {
////                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 2, 4, 8 seconds
////                    _logger.LogWarning(ex, "Rate limit hit, retrying in {Delay} seconds. Attempt {Attempt}/3", delay.TotalSeconds, attempt);
////                    await Task.Delay(delay);
////                }
////            }

////            throw new InvalidOperationException("Failed to make request after 3 attempts due to rate limiting");
////        }
////        finally
////        {
////            _rateLimitSemaphore.Release();
////        }
////    }

////    private TaggingResult GenerateFallbackTags(string eventName, string description)
////    {
////        var fallbackTags = new List<string>();

////        // Add basic tags based on event name
////        var lowerName = eventName.ToLower();
////        if (lowerName.Contains("концерт")) fallbackTags.Add("концерт");
////        if (lowerName.Contains("театър")) fallbackTags.Add("театър");
////        if (lowerName.Contains("изложба")) fallbackTags.Add("изложба");
////        if (lowerName.Contains("фестивал")) fallbackTags.Add("фестивал");
////        if (lowerName.Contains("спорт")) fallbackTags.Add("спорт");
////        if (lowerName.Contains("бизнес")) fallbackTags.Add("бизнес");

////        // Add generic location tag
////        fallbackTags.Add("софия");

////        if (!fallbackTags.Any())
////            fallbackTags.Add("събитие");

////        return new TaggingResult
////        {
////            SuggestedCategory = EventCategory.Exhibitions,
////            SuggestedTags = fallbackTags,
////            Confidence = fallbackTags.ToDictionary(tag => tag, _ => 0.5)
////        };
////    }

////    public void Dispose()
////    {
////        _rateLimitSemaphore?.Dispose();
////    }
////}

////// Response models for OpenAI API
////public class OpenAiResponse
////{
////    [JsonPropertyName("choices")]
////    public OpenAiChoice[]? Choices { get; set; }
////}

////public class OpenAiChoice
////{
////    [JsonPropertyName("message")]
////    public OpenAiMessage? Message { get; set; }
////}

////public class OpenAiMessage
////{
////    [JsonPropertyName("content")]
////    public string? Content { get; set; }
////}

////public class AiTaggingResponse
////{
////    [JsonPropertyName("category")]
////    public int Category { get; set; }

////    [JsonPropertyName("subcategory")]
////    public string? Subcategory { get; set; }

////    [JsonPropertyName("tags")]
////    public List<string>? Tags { get; set; }
////}




//using Events.Crawler.Models;
//using Events.Crawler.Services.Interfaces;
//using Events.Models.Enums;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Events.Crawler.Services.Implementations;

//public class OpenAiTaggingService : IAiTaggingService
//{
//    private readonly HttpClient _http;
//    private readonly string _apiKey;
//    private readonly ILogger<OpenAiTaggingService> _logger;

//    public OpenAiTaggingService(HttpClient http, IConfiguration config, ILogger<OpenAiTaggingService> logger)
//    {
//        _http = http;
//        _apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");
//        _logger = logger;

//        _http.BaseAddress = new Uri("https://api.openai.com/");
//        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
//    }

//    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $@"Анализирай следното събитие и определи категорията му. Върни само числото на категорията.

//Заглавие: {eventName}
//Описание: {description}

//Категории:
//1 - Music (музика)
//2 - Art (изкуство) 
//3 - Business (бизнес)
//4 - Sports (спорт)
//5 - Theatre (театър)
//6 - Cinema (кино)
//7 - Festivals (фестивали)
//8 - Exhibitions (изложби)
//9 - Conferences (конференции)
//10 - Workshops (работилници)

//Върни само числото на най-подходящата категория (1-10):";

//            var request = new
//            {
//                model = "gpt-3.5-turbo",
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                },
//                max_tokens = 10,
//                temperature = 0.1
//            };

//            var response = await _http.PostAsJsonAsync("v1/chat/completions", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
//            var categoryText = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//            if (int.TryParse(categoryText, out var categoryId) &&
//                Enum.IsDefined(typeof(EventCategory), categoryId))
//            {
//                return (EventCategory)categoryId;
//            }

//            _logger.LogWarning("Failed to parse category from OpenAI response: {Response}", categoryText);
//            return EventCategory.Exhibitions; // Default fallback
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error classifying event with OpenAI API");
//            return EventCategory.Exhibitions; // Default fallback
//        }
//    }

//    public async Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
//    {
//        try
//        {
//            var prompt = $@"Анализирай следното събитие в София и предложи категория, подкатегория и тагове:

//Заглавие: {eventName}
//Описание: {description}
//Местоположение: {location ?? "София"}

//Върни отговора в JSON формат със следната структура:
//{{
//    ""category"": число от 1 до 10 (виж категориите по-долу),
//    ""subcategory"": конкретна подкатегория като стринг,
//    ""tags"": масив от 3-7 тагове на български език
//}}

//Категории:
//1=Music, 2=Art, 3=Business, 4=Sports, 5=Theatre, 6=Cinema, 7=Festivals, 8=Exhibitions, 9=Conferences, 10=Workshops

//Музикални подкатегории: рок, джаз, метъл, поп, фънк, пънк, опера, класическа, електронна, фолк, блус, кънтри, регей, хип-хоп, алтернативна

//Спортни подкатегории: футбол, баскетбол, тенис, волейбол, плуване, атлетика, бокс, борба, гимнастика, колоездене

//Таговете да включват: жанр/тип, настроение, възрастова група, време на деня, цена, стил и др.";

//            var request = new
//            {
//                model = "gpt-3.5-turbo",
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                },
//                max_tokens = 300,
//                temperature = 0.3
//            };

//            var response = await _http.PostAsJsonAsync("v1/chat/completions", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
//            var content = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//            if (!string.IsNullOrEmpty(content))
//            {
//                try
//                {
//                    var aiResult = JsonSerializer.Deserialize<AiTaggingResponse>(content, new JsonSerializerOptions
//                    {
//                        PropertyNameCaseInsensitive = true
//                    });

//                    if (aiResult != null)
//                    {
//                        var category = Enum.IsDefined(typeof(EventCategory), aiResult.Category)
//                            ? (EventCategory)aiResult.Category
//                            : EventCategory.Exhibitions;

//                        var tags = aiResult.Tags?.Where(t => !string.IsNullOrWhiteSpace(t)).ToList() ?? new List<string>();

//                        // Add subcategory as a tag if provided
//                        if (!string.IsNullOrWhiteSpace(aiResult.Subcategory))
//                        {
//                            tags.Insert(0, aiResult.Subcategory);
//                        }

//                        return new TaggingResult
//                        {
//                            SuggestedCategory = category,
//                            SuggestedTags = tags,
//                            Confidence = tags.ToDictionary(tag => tag, _ => 0.8)
//                        };
//                    }
//                }
//                catch (JsonException ex)
//                {
//                    _logger.LogWarning(ex, "Failed to parse OpenAI JSON response: {Content}", content);

//                    // Fallback parsing
//                    return GenerateFallbackTags(eventName, description);
//                }
//            }

//            return GenerateFallbackTags(eventName, description);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error generating tags with OpenAI API");
//            return GenerateFallbackTags(eventName, description);
//        }
//    }

//    public async Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
//    {
//        try
//        {
//            var prompt = $@"Анализирай следното музикално събитие и определи музикалните жанрове:

//Заглавие: {eventName}
//Описание: {description}

//Музикални жанрове: рок, джаз, метъл, поп, фънк, пънк, опера, класическа, електронна, фолк, блус, кънтри, регей, хип-хоп, алтернативна

//Върни до 3 най-точни жанра като JSON масив от стрингове:";

//            var request = new
//            {
//                model = "gpt-3.5-turbo",
//                messages = new[]
//                {
//                    new { role = "user", content = prompt }
//                },
//                max_tokens = 100,
//                temperature = 0.2
//            };

//            var response = await _http.PostAsJsonAsync("v1/chat/completions", request);
//            response.EnsureSuccessStatusCode();

//            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
//            var content = result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim();

//            if (!string.IsNullOrEmpty(content))
//            {
//                try
//                {
//                    var genres = JsonSerializer.Deserialize<string[]>(content);
//                    return genres ?? new[] { "музика" };
//                }
//                catch (JsonException)
//                {
//                    // Fallback parsing
//                    return content
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
//            _logger.LogError(ex, "Error extracting music genres with OpenAI API");
//            return new[] { "музика" };
//        }
//    }

//    private TaggingResult GenerateFallbackTags(string eventName, string description)
//    {
//        var fallbackTags = new List<string>();

//        // Add basic tags based on event name
//        var lowerName = eventName.ToLower();
//        if (lowerName.Contains("концерт")) fallbackTags.Add("концерт");
//        if (lowerName.Contains("театър")) fallbackTags.Add("театър");
//        if (lowerName.Contains("изложба")) fallbackTags.Add("изложба");
//        if (lowerName.Contains("фестивал")) fallbackTags.Add("фестивал");
//        if (lowerName.Contains("спорт")) fallbackTags.Add("спорт");
//        if (lowerName.Contains("бизнес")) fallbackTags.Add("бизнес");

//        // Add generic location tag
//        fallbackTags.Add("софия");

//        if (!fallbackTags.Any())
//            fallbackTags.Add("събитие");

//        return new TaggingResult
//        {
//            SuggestedCategory = EventCategory.Exhibitions,
//            SuggestedTags = fallbackTags,
//            Confidence = fallbackTags.ToDictionary(tag => tag, _ => 0.5)
//        };
//    }
//}

//// Response models for OpenAI API
//public class OpenAiResponse
//{
//    [JsonPropertyName("choices")]
//    public OpenAiChoice[]? Choices { get; set; }
//}

//public class OpenAiChoice
//{
//    [JsonPropertyName("message")]
//    public OpenAiMessage? Message { get; set; }
//}

//public class OpenAiMessage
//{
//    [JsonPropertyName("content")]
//    public string? Content { get; set; }
//}

//public class AiTaggingResponse
//{
//    [JsonPropertyName("category")]
//    public int Category { get; set; }

//    [JsonPropertyName("subcategory")]
//    public string? Subcategory { get; set; }

//    [JsonPropertyName("tags")]
//    public List<string>? Tags { get; set; }
//}
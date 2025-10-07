using Events.Crawler.Models;
using Events.Crawler.Services.Interfaces;
using Events.Models.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Events.Crawler.Services.Implementations;

public class ClaudeApiClient : IAiTaggingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public ClaudeApiClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Claude:ApiKey"]; // TODO:
    }

    public async Task<EventCategory> ClassifyEventAsync(string eventName, string description)
    {
        var prompt = $"""
        Категоризирай събитието:
        Заглавие: {eventName}
        Описание: {description}
        Върни жанр, възрастова група и 3 тагове.
        """;

        var request = new
        {
            model = "claude-2",
            prompt = prompt,
            temperature = 0.7,
            max_tokens = 300
        };

        var response = await _http.PostAsJsonAsync("https://api.anthropic.com/v1/completions", request);
        //var result = await response.Content.ReadFromJsonAsync<ClaudeResponse>();
        // TODO: 

        throw new NotImplementedException();
    }

    public Task<IEnumerable<string>> ExtractMusicGenresAsync(string eventName, string description)
    {
        throw new NotImplementedException();
    }

    public Task<TaggingResult> GenerateTagsAsync(string eventName, string description, string? location = null)
    {
        throw new NotImplementedException();
    }
}

using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Events.Crawler.Enums;
using Events.Crawler.DTOs.Bilet;
using Events.Crawler.DTOs.Common;

namespace Events.Crawler.Services.Implementations;

public class BiletBgCrawler : IHttpApiCrawler
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BiletBgCrawler> _logger;

    public string SourceName => "bilet.bg";
    public CrawlerType CrawlerType => CrawlerType.HttpApi;

    public BiletBgCrawler(HttpClient httpClient, IConfiguration configuration, ILogger<BiletBgCrawler> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri("https://panel.bilet.bg/api/v1/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Events-Crawler/1.0");
    }

    public async Task<CrawlResult> CrawlAsync(DateTime? targetDate = null)
    {
        var result = new CrawlResult
        {
            Source = SourceName,
            CrawledAt = DateTime.UtcNow
        };

        var startTime = DateTime.UtcNow;

        try
        {
            var currentDate = (targetDate ?? DateTime.Now).ToString("yyyy-MM-dd");
            var events = await GetAllEventsForDate(currentDate);

            var sofiaEvents = events
                .Select(MapToStandardDto)
                .Where(e => e != null) // Filter out null (non-Sofia) events
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = events.Count; // Total found events
            result.Events = sofiaEvents; // Only Sofia events
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} events from Bilet.bg, {SofiaEvents} in Sofia",
                events.Count, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling Bilet.bg");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            result.Duration = DateTime.UtcNow - startTime;
        }

        return result;
    }

    public async Task<T> GetDataAsync<T>(string endpoint, Dictionary<string, string>? parameters = null)
    {
        var queryString = parameters != null 
            ? "?" + string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"))
            : "";

        var response = await _httpClient.GetAsync($"{endpoint}{queryString}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public bool IsHealthy()
    {
        try
        {
            return _httpClient.BaseAddress != null;
        }
        catch
        {
            return false;
        }
    }

    private async Task<List<BiletEventDto>> GetAllEventsForDate(string date)
    {
        var allEvents = new List<BiletEventDto>();
        var parameters = new Dictionary<string, string>
        {
            ["sort"] = "start_date",
            ["filter[end_date]"] = date,
            ["per_page"] = "15",
            ["include"] = "venue",
            ["page"] = "1"
        };

        string? nextPageUrl = $"events?{string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"))}";

        do
        {
            var ticket = await GetDataAsync<BiletTicketDto>(nextPageUrl);
            if (ticket?.Events != null)
                allEvents.AddRange(ticket.Events);
                        
            nextPageUrl = ticket?.NextPageUrl;
        }
        while (!string.IsNullOrEmpty(nextPageUrl));

        return allEvents;
    }

    private CrawledEventDto? MapToStandardDto(BiletEventDto biletEvent)
    {
        // Early filtration for Events because we are interested in Events from Sofia only
        var city = biletEvent.Place?.City?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(city) || !IsSofiaCity(city))
        {
            _logger.LogDebug("Filtering out non-Sofia event: {EventName} in {City}",
                biletEvent.Name, biletEvent.Place?.City);
            return null; // Skip non-Sofia events
        }

        return new CrawledEventDto
        {
            ExternalId = biletEvent.Id.ToString(),
            Source = SourceName,
            Name = biletEvent.Name ?? "",
            Description = biletEvent.Description,
            Location = $"{biletEvent.Place?.City} {biletEvent.Place?.Address} {biletEvent.Place?.Name}",
            StartDate = TryParseDate(biletEvent.StartDate),
            EndDate = TryParseDate(biletEvent.EndDate),
            ImageUrl = biletEvent.Image,
            SourceUrl = !string.IsNullOrEmpty(biletEvent.Slug)
                ? $"https://bilet.bg/event/{biletEvent.Slug}"
                : null,
            RawData = new Dictionary<string, object>
            {
                ["bilet_id"] = biletEvent.Id,
                ["slug"] = biletEvent.Slug ?? "",
                ["created_at"] = biletEvent.CreatedAt ?? ""
            }
        };
    }

    private static bool IsSofiaCity(string city)
    {
        var sofiaCities = new[]
        {
        "софия", "sofia", "софија", "sofija",
        "гр. софия", "гр.София", "sofia city"
    };

        return sofiaCities.Any(sc => city.Contains(sc));
    }

    private DateTime? TryParseDate(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString)) return null;
        return DateTime.TryParse(dateString, out var date) ? date : null;
    }
}
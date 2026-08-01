using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Events.Crawler.Enums;
using Events.Crawler.DTOs.Paysera;
using Events.Crawler.DTOs.Common;

namespace Events.Crawler.Services.Implementations;

public class PayseraApiCrawler : IHttpApiCrawler
{
    private const string BaseUrl = "https://tickets.paysera.com/";
    private const string EventsEndpoint = "bg-BG/ticket-frontend/rest/v1/public/events";
    private const string SofiaWhere = "София, България";
    private const string SofiaPlaceId = "ChIJ9Xsxy4KGqkARYF6_aRKgAAQ";
    private const int PageLimit = 100;

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PayseraApiCrawler> _logger;

    public string SourceName => "tickets.paysera.com";
    public CrawlerType CrawlerType => CrawlerType.HttpApi;

    public PayseraApiCrawler(HttpClient httpClient, IConfiguration configuration, ILogger<PayseraApiCrawler> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(BaseUrl);
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
            var events = await GetAllSofiaEvents();

            var mappedEvents = events
                .Select(MapToStandardDto)
                .Where(e => e != null) // Filter out disabled/sold out events
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = events.Count;
            result.Events = mappedEvents;
            result.EventsProcessed = mappedEvents.Count;
            result.Success = true;

            _logger.LogInformation("[{Source}] Crawled {TotalEvents} events from Paysera, {MappedEvents} available", SourceName, events.Count, mappedEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Source}] Error crawling Paysera", SourceName);
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

    private async Task<List<PayseraEventDto>> GetAllSofiaEvents()
    {
        var allEvents = new List<PayseraEventDto>();
        var offset = 0;
        var total = int.MaxValue;

        while (offset < total)
        {
            var parameters = new Dictionary<string, string>
            {
                ["where"] = SofiaWhere,
                ["place_id"] = SofiaPlaceId,
                ["limit"] = PageLimit.ToString(),
                ["offset"] = offset.ToString()
            };

            var response = await GetDataAsync<PayseraEventsResponseDto>(EventsEndpoint, parameters);
            if (response?.Items != null)
                allEvents.AddRange(response.Items);

            total = response?.Total ?? allEvents.Count;
            offset += PageLimit;
        }

        return allEvents;
    }

    private CrawledEventDto? MapToStandardDto(PayseraEventDto payseraEvent)
    {
        if (!payseraEvent.Enabled || payseraEvent.SoldOut)
        {
            _logger.LogDebug("[{Source}] Filtering out disabled/sold out event: {EventName} ({EventId})", SourceName, payseraEvent.Name, payseraEvent.Id);
            return null;
        }

        var price = TryParsePrice(payseraEvent.PriceFrom);

        return new CrawledEventDto
        {
            ExternalId = payseraEvent.Id.ToString(),
            Source = SourceName,
            Name = payseraEvent.Name ?? "",
            Description = payseraEvent.Description,
            City = "софия",
            Location = payseraEvent.LocationData?.Name ?? payseraEvent.Location,
            StartDate = TryParseUnixDate(payseraEvent.DateStarts),
            EndDate = TryParseUnixDate(payseraEvent.DateEnds),
            ImageUrl = !string.IsNullOrEmpty(payseraEvent.MainImage) ? $"{BaseUrl.TrimEnd('/')}{payseraEvent.MainImage}" : null,
            SourceUrl = BaseUrl,
            TicketUrl = payseraEvent.Url,
            Price = price,
            IsFree = price == 0, // Paysera gives an unambiguous price_from, so we can determine this precisely instead of defaulting to paid
            RawData = new Dictionary<string, object>
            {
                ["paysera_id"] = payseraEvent.Id,
                ["slug"] = payseraEvent.Slug ?? "",
                ["category_name"] = payseraEvent.Category?.Name ?? "",
                ["enabled"] = payseraEvent.Enabled,
                ["sold_out"] = payseraEvent.SoldOut
            }
        };
    }

    private static DateTime? TryParseUnixDate(long? unixSeconds)
    {
        if (unixSeconds == null) return null;
        return DateTimeOffset.FromUnixTimeSeconds(unixSeconds.Value).DateTime;
    }

    private static decimal? TryParsePrice(string? priceString)
    {
        if (string.IsNullOrEmpty(priceString)) return null;
        return decimal.TryParse(priceString, System.Globalization.CultureInfo.InvariantCulture, out var price) ? price : null;
    }
}

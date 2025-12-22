using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.Eventim;
using Events.Crawler.Enums;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class EventimCrawler : IWebScrapingCrawler
{
    private readonly ILogger<EventimCrawler> _logger;
    private readonly int _maxRetries = 3;
    private readonly int _delayBetweenRequests = 500;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    public string SourceName => "eventim.bg";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public EventimCrawler(ILogger<EventimCrawler> logger)
    {
        _logger = logger;
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
            EnsureBrowsersInstalled();

            var eventimEvents = await GetEventimEventsAsync();

            var sofiaEvents = eventimEvents
                .Select(MapEventimToStandardDto)
                .Where(e => e != null)
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = eventimEvents.Count;
            result.Events = sofiaEvents;
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} events from Eventim, {SofiaEvents} in Sofia", eventimEvents.Count, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling Eventim.bg");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            result.Duration = DateTime.UtcNow - startTime;
        }

        return result;
    }

    public async Task<IEnumerable<string>> ExtractElementsAsync(string url, string selector)
    {
        try
        {
            EnsureBrowsersInstalled();

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
            });

            var page = await browser.NewPageAsync();

            try
            {
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 60000
                });

                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                var elements = await page.QuerySelectorAllAsync(selector);
                var results = new List<string>();

                foreach (var element in elements)
                {
                    var text = await element.InnerTextAsync();
                    if (!string.IsNullOrWhiteSpace(text))
                        results.Add(text.Trim());
                }

                return results;
            }
            finally
            {
                await browser.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting elements from {Url} with selector {Selector}", url, selector);
            throw;
        }
    }

    public async Task<string> GetPageContentAsync(string url)
    {
        try
        {
            EnsureBrowsersInstalled();

            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
            });

            var page = await browser.NewPageAsync();

            try
            {
                await page.GotoAsync(url, new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 60000
                });
                return await page.ContentAsync();
            }
            finally
            {
                await browser.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting page content from {Url}", url);
            throw;
        }
    }

    public bool IsHealthy()
    {
        try
        {
            var chromiumPath = GetChromiumPath();
            return !string.IsNullOrEmpty(chromiumPath) && File.Exists(chromiumPath);
        }
        catch
        {
            return false;
        }
    }

    private async Task<List<EventimEventInstance>> GetEventimEventsAsync()
    {
        var retryCount = 0;

        while (retryCount < _maxRetries)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[] {
                        "--disable-blink-features=AutomationControlled",
                        "--disable-dev-shm-usage",
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-web-security",
                        "--disable-features=VizDisplayCompositor"
                    }
                });

                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    Locale = "bg-BG",
                    TimezoneId = "Europe/Sofia",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                    ExtraHTTPHeaders = new Dictionary<string, string>
                    {
                        ["Accept-Language"] = "bg-BG,bg;q=0.9,en;q=0.8",
                        ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
                        ["Accept-Encoding"] = "gzip, deflate, br"
                    }
                });

                var page = await context.NewPageAsync();

                try
                {
                    // Add script to avoid automation detection
                    await page.AddInitScriptAsync(@"
                        Object.defineProperty(navigator, 'webdriver', {
                            get: () => undefined,
                        });
                        
                        if (window.chrome && window.chrome.runtime && window.chrome.runtime.onConnect) {
                            delete window.chrome.runtime.onConnect;
                        }
                        
                        Object.defineProperty(navigator, 'plugins', {
                            get: () => [1, 2, 3, 4, 5],
                        });
                    ");

                    _logger.LogInformation("Loading Eventim main page...");

                    await page.GotoAsync("https://www.eventim.bg/bg/tursi/?lang=bg", new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        Timeout = 60000
                    });

                    _logger.LogDebug("Page loaded successfully");
                    await Task.Delay(3000);

                    // Simulate human behavior
                    await page.Mouse.MoveAsync(300, 200, new MouseMoveOptions { Steps = 10 });
                    await Task.Delay(1000);

                    for (int i = 0; i < 3; i++)
                    {
                        await page.Mouse.WheelAsync(0, 300);
                        await Task.Delay(1000);
                    }

                    // Now crawl all events
                    var allEvents = await CrawlAllEventsAsync(page);
                    return allEvents;
                }
                finally
                {
                    await browser.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "Attempt {Retry} failed for Eventim crawl", retryCount);

                if (retryCount >= _maxRetries)
                    throw;

                await Task.Delay(_delayBetweenRequests * retryCount);
            }
        }

        return new List<EventimEventInstance>();
    }

    private async Task<List<EventimEventInstance>> CrawlAllEventsAsync(IPage page)
    {
        var allEvents = new List<EventimEventInstance>();
        var processedUrls = new HashSet<string>();

        _logger.LogInformation("Starting to crawl all Eventim events...");

        var currentUrl = "/api/search?lang=bg";
        int consecutiveEmptyPages = 0;
        const int maxEmptyPages = 3;
        int totalProcessed = 0;

        while (!string.IsNullOrEmpty(currentUrl) && !processedUrls.Contains(currentUrl))
        {
            processedUrls.Add(currentUrl);
            totalProcessed++;

            try
            {
                _logger.LogDebug("API call to: {Url} (page {PageNumber})", currentUrl, totalProcessed);

                var apiResponse = await page.EvaluateAsync<string>($@"
                    async () => {{
                        try {{
                            const response = await fetch('{currentUrl}', {{
                                method: 'GET',
                                headers: {{
                                    'Accept': 'application/json',
                                    'Content-Type': 'application/json'
                                }}
                            }});
                            
                            if (response.ok) {{
                                return await response.text();
                            }} else {{
                                return `Error: ${{response.status}} ${{response.statusText}}`;
                            }}
                        }} catch (error) {{
                            return `Fetch error: ${{error.message}}`;
                        }}
                    }}
                ");

                if (string.IsNullOrEmpty(apiResponse) || apiResponse.StartsWith("Error") || apiResponse.StartsWith("Fetch error"))
                {
                    _logger.LogWarning("API call error: {Error}", apiResponse);
                    break;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                EventimResultsDto? parsed = null;
                try
                {
                    parsed = JsonSerializer.Deserialize<EventimResultsDto>(apiResponse, options);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogWarning(jsonEx, "JSON parsing error, attempting to clean response");
                    var cleanedJson = CleanJsonCategoryFields(apiResponse);
                    parsed = JsonSerializer.Deserialize<EventimResultsDto>(cleanedJson, options);
                }

                if (parsed?.Results != null && parsed.Results.Count > 0)
                {
                    _logger.LogDebug("Received {EventCount} events from this page", parsed.Results.Count);

                    var eventInstances = ConvertToEventInstances(parsed.Results);
                    var sofiaEventsCount = eventInstances.Count;
                    allEvents.AddRange(eventInstances);

                    _logger.LogDebug("Sofia events from this page: {SofiaEvents}, Total: {TotalEvents}", sofiaEventsCount, allEvents.Count);

                    consecutiveEmptyPages = 0;
                }
                else
                {
                    _logger.LogDebug("No events from this page");
                    consecutiveEmptyPages++;

                    if (consecutiveEmptyPages >= maxEmptyPages)
                    {
                        _logger.LogInformation("Stopping after {EmptyPages} consecutive empty pages", consecutiveEmptyPages);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(parsed?.NextPage))
                {
                    var uri = new Uri(parsed.NextPage);
                    currentUrl = uri.PathAndQuery;
                    _logger.LogDebug("Next page: {NextUrl}", currentUrl);
                    await Task.Delay(_delayBetweenRequests);
                }
                else
                {
                    _logger.LogInformation("No more pages available");
                    break;
                }

                if (totalProcessed > 200) // Safety limit
                {
                    _logger.LogWarning("Reached maximum page limit - stopping");
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing page {Url}", currentUrl);
                break;
            }
        }

        _logger.LogInformation("Total collected Sofia events: {TotalEvents} (from {TotalPages} pages)", allEvents.Count, totalProcessed);
        return allEvents;
    }

    private string CleanJsonCategoryFields(string json)
    {
        var pattern = @"""category"":\s*""(\d+)""";
        var replacement = @"""category"":$1";
        return Regex.Replace(json, pattern, replacement);
    }

    private List<EventimEventInstance> ConvertToEventInstances(List<EventimEventDto> eventimEvents)
    {
        var eventInstances = new List<EventimEventInstance>();

        foreach (var eventimEvent in eventimEvents)
        {
            if (eventimEvent.Events != null && eventimEvent.Events.Any())
            {
                foreach (var eventInstance in eventimEvent.Events)
                {
                    // Filter: Only Sofia events
                    if (!IsSofiaEvent(eventInstance.City))
                        continue;

                    var eventInfo = new EventimEventInstance
                    {
                        Name = eventimEvent.Title,
                        Description = null, // Not provided by API
                        ImageUrl = !string.IsNullOrEmpty(eventimEvent.Image) ? $"https://www.eventim.bg{eventimEvent.Image}" : null,
                        Date = eventInstance.DateStart?.DateTime,
                        Price = null, // Ignore the price for now
                        IsFree = false, // by default all events are considered paid. The admin will adjust if needed the free ones
                        City = eventInstance.City,
                        TicketUrl = !string.IsNullOrEmpty(eventimEvent.Url) ? $"https://www.eventim.bg{eventimEvent.Url}" : null,
                        SourceUrl = "https://www.eventim.bg",
                        Venue = eventInstance.Venue
                    };

                    eventInstances.Add(eventInfo);
                }
            }
        }

        return eventInstances;
    }

    private bool IsSofiaEvent(string? city)
    {
        if (string.IsNullOrEmpty(city))
            return false;

        var sofiaVariants = new[]
        {
            "София", "Sofia", "СОФИЯ", "SOFIA",
            "Софія", "Софија", // Different cyrillic variants
            "ГОЛЯМА СЦЕНА", // Special case - theaters in Sofia
            "Голяма сцена"
        };

        return sofiaVariants.Any(variant => city.Contains(variant, StringComparison.OrdinalIgnoreCase));
    }

    private CrawledEventDto? MapEventimToStandardDto(EventimEventInstance eventimEvent)
    {
        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(eventimEvent.Name, eventimEvent.TicketUrl),
            Source = SourceName,
            Name = CleanText(eventimEvent.Name) ?? "Unknown Event",
            Description = CleanText(eventimEvent.Description),
            StartDate = TryParseEventDate(eventimEvent.Date),
            City = eventimEvent.City ?? "",
            Location = eventimEvent.Venue ?? "",
            ImageUrl = eventimEvent.ImageUrl,
            SourceUrl = eventimEvent.SourceUrl,
            TicketUrl = eventimEvent.TicketUrl,
            Price = eventimEvent.Price,
            IsFree = eventimEvent.IsFree,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = eventimEvent.Date ?? "",
                ["city"] = eventimEvent.City ?? "",
                ["venue"] = eventimEvent.Venue ?? "",
                ["extraction_method"] = "Playwright_Eventim_API",
                ["crawled_from"] = "https://www.eventim.bg/api/search"
            }
        };
    }

    private string GenerateEventId(string? title, string? url)
    {
        var combined = $"{title}|{url ?? ""}";
        return Math.Abs(combined.GetHashCode()).ToString();
    }

    private string? CleanText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        return text.Trim().Replace("\n", " ").Replace("\r", "").Replace("  ", " ");
    }

    private DateTime? TryParseEventDate(string? dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText)) return null;

        var formats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-dd HH:mm:ss",
            "dd.MM.yyyy",
            "dd/MM/yyyy",
            "yyyy-MM-dd",
            "dd MMMM yyyy",
            "dd MMM yyyy",
            "dd.MM.yyyy HH:mm",
            "dd/MM/yyyy HH:mm"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateText.Trim(), format, null, System.Globalization.DateTimeStyles.None, out var date))
            {
                return date;
            }
        }

        if (DateTime.TryParse(dateText, out var parsedDate))
        {
            return parsedDate;
        }

        _logger.LogDebug("Could not parse date: {DateText}", dateText);
        return null;
    }

    private void EnsureBrowsersInstalled()
    {
        if (_browsersInstalled) return;

        lock (_installLock)
        {
            if (_browsersInstalled) return;

            try
            {
                _logger.LogInformation("Checking Playwright browser installation...");

                var chromiumPath = GetChromiumPath();
                if (string.IsNullOrEmpty(chromiumPath) || !File.Exists(chromiumPath))
                {
                    _logger.LogWarning("Playwright browsers not found. Attempting to install...");
                    InstallPlaywrightBrowsers();
                }

                _browsersInstalled = true;
                _logger.LogInformation("Playwright browsers are ready");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure Playwright browsers are installed");
                throw new InvalidOperationException(
                    "Playwright browsers are not installed. Please run 'npx playwright install chromium' manually.", ex);
            }
        }
    }

    private void InstallPlaywrightBrowsers()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "npx",
                Arguments = "playwright install chromium",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process != null)
            {
                process.WaitForExit(TimeSpan.FromMinutes(5));

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    throw new InvalidOperationException($"Failed to install Playwright browsers: {error}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error installing Playwright browsers");
            throw;
        }
    }

    private string? GetChromiumPath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var playwrightDir = Path.Combine(localAppData, "ms-playwright");

        if (!Directory.Exists(playwrightDir)) return null;

        var chromiumDirs = Directory.GetDirectories(playwrightDir, "chromium-*");
        if (!chromiumDirs.Any()) return null;

        var latestChromiumDir = chromiumDirs.OrderByDescending(d => d).First();
        var chromePath = Path.Combine(latestChromiumDir, "chrome-win", "chrome.exe");

        return File.Exists(chromePath) ? chromePath : null;
    }

    // Helper class for event instances
    private class EventimEventInstance
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Date { get; set; }
        public decimal? Price { get; set; }
        public string? City { get; set; }
        public string? TicketUrl { get; set; }
        public string? SourceUrl { get; set; }
        public bool IsFree { get; set; }
        public string? Venue { get; set; }
    }
}
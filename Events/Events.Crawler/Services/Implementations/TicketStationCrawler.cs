using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.TicketStation;
using Events.Crawler.Enums;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;

namespace Events.Crawler.Services.Implementations;

public class TicketStationCrawler : IWebScrapingCrawler
{
    private readonly ILogger<TicketStationCrawler> _logger;
    private readonly int _maxRetries = 3;
    private readonly int _delayBetweenRequests = 2000;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    public string SourceName => "ticketstation.bg";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public TicketStationCrawler(ILogger<TicketStationCrawler> logger)
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

            var ticketStationEvents = await GetTicketStationEventsAsync("https://ticketstation.bg/bg/attractions");

            var sofiaEvents = ticketStationEvents
                .Select(MapTicketStationToStandardDto)
                .Where(e => e != null) // Filter out null (non-Sofia) events
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = ticketStationEvents.Count; // Total found events
            result.Events = sofiaEvents; // Only Sofia events
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} events from TicketStation, {SofiaEvents} in Sofia",
                ticketStationEvents.Count, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling TicketStation.bg");
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
            // Check if Playwright browsers are installed
            var chromiumPath = GetChromiumPath();
            return !string.IsNullOrEmpty(chromiumPath) && File.Exists(chromiumPath);
        }
        catch
        {
            return false;
        }
    }


    private async Task<List<TicketStationEventDto>> GetTicketStationEventsAsync(string url)
    {
        var retryCount = 0;

        while (retryCount < _maxRetries)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();

                try
                {
                    // Load the page
                    await page.GotoAsync(url, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.DOMContentLoaded,
                        Timeout = 60000
                    });

                    // Wait for the item cards to load
                    try
                    {
                        await page.WaitForSelectorAsync(".item", new PageWaitForSelectorOptions { Timeout = 15000 });
                    }
                    catch (TimeoutException)
                    {
                        _logger.LogWarning("Timeout waiting for .item elements");
                        await Task.Delay(5000); // Fallback wait
                    }

                    // Additional wait for dynamic content
                    await Task.Delay(3000);

                    // Get all item cards
                    var itemCards = await page.QuerySelectorAllAsync(".item");
                    var events = new List<TicketStationEventDto>();

                    _logger.LogInformation("Found {ItemCount} item cards", itemCards.Count);

                    foreach (var card in itemCards)
                    {
                        try
                        {
                            var eventDto = new TicketStationEventDto();

                            // Get the item-info container
                            var itemInfo = await card.QuerySelectorAsync(".item-info");
                            if (itemInfo == null)
                            {
                                continue;
                            }

                            // Extract name from .item-name
                            var nameElement = await itemInfo.QuerySelectorAsync(".item-name");
                            if (nameElement != null)
                            {
                                eventDto.Name = await nameElement.InnerTextAsync();
                            }

                            // Extract date from .item-date
                            var dateElement = await itemInfo.QuerySelectorAsync(".item-date");
                            if (dateElement != null)
                            {
                                eventDto.Date = await dateElement.InnerTextAsync();
                            }

                            // Extract city from .item-city
                            var cityElement = await itemInfo.QuerySelectorAsync(".item-city");
                            if (cityElement != null)
                            {
                                eventDto.City = await cityElement.InnerTextAsync();
                            }

                            // Extract image URL from the card
                            var imageElement = await card.QuerySelectorAsync("img");
                            if (imageElement != null)
                            {
                                eventDto.ImageUrl = await imageElement.GetAttributeAsync("src");
                                // Make URL absolute if it's relative
                                if (!string.IsNullOrEmpty(eventDto.ImageUrl) && eventDto.ImageUrl.StartsWith("/"))
                                {
                                    var baseUri = new Uri(url);
                                    eventDto.ImageUrl = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.ImageUrl}";
                                }
                            }

                            // Extract URL if there's a link in the card
                            var linkElement = await card.QuerySelectorAsync("a[href]");
                            if (linkElement != null)
                            {
                                eventDto.Url = await linkElement.GetAttributeAsync("href");
                                // Make URL absolute if it's relative
                                if (!string.IsNullOrEmpty(eventDto.Url) && eventDto.Url.StartsWith("/"))
                                {
                                    var baseUri = new Uri(url);
                                    eventDto.Url = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.Url}";
                                }
                            }
                            else
                            {
                                // Fallback to source URL
                                eventDto.Url = url;
                            }

                            // Set description from item-info content
                            eventDto.Description = await itemInfo.InnerTextAsync();

                            //// TODO: get the prive information from the description. It is between "..." simbol and ends with "EUR symbol
                            //var pFrom = eventDto.Description.IndexOf("... ") + "... ".Length;
                            //var pTo = eventDto.Description.LastIndexOf("EUR ") + 4;
                            //var price = eventDto.Description.Substring(pFrom, pTo - pFrom);
                            //eventDto.Price = price; // TODO: Clarify in witch currency will be shown or make price string instead of decimal 

                            // Only add events that have at least a name
                            if (!string.IsNullOrEmpty(eventDto.Name))
                            {
                                events.Add(eventDto);

                                _logger.LogDebug("Extracted event: {EventName} on {EventDate} in {EventCity}",
                                    eventDto.Name, eventDto.Date, eventDto.City);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error extracting event data from card");
                        }
                    }

                    return events;
                }
                finally
                {
                    await browser.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "Attempt {Retry} failed for TicketStation crawl", retryCount);

                if (retryCount >= _maxRetries)
                    throw;

                await Task.Delay(_delayBetweenRequests * retryCount);
            }
        }

        return new List<TicketStationEventDto>();
    }

    private CrawledEventDto? MapTicketStationToStandardDto(TicketStationEventDto ticketStationEvent)
    {
        // Early filtration for Events because we are interested in Events from Sofia only
        var city = ticketStationEvent.City?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(city) || !IsSofiaCity(city))
        {
            _logger.LogDebug("Filtering out non-Sofia event: {EventName} in {City}",
                ticketStationEvent.Name, ticketStationEvent.City);
            return null; // Skip non-Sofia events
        }

        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(ticketStationEvent.Name, ticketStationEvent.Url),
            Source = SourceName,
            Name = CleanText(ticketStationEvent.Name) ?? "Unknown Event",
            Description = CleanText(ticketStationEvent.Description),
            StartDate = TryParseEventDate(ticketStationEvent.Date),
            Location = CleanText(ticketStationEvent.City),
            ImageUrl = ticketStationEvent.ImageUrl,
            SourceUrl = ticketStationEvent.Url,
            Price = ticketStationEvent.Price,
            IsFree = ticketStationEvent.Price == null || ticketStationEvent.Price == 0,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = ticketStationEvent.Date ?? "",
                ["city"] = ticketStationEvent.City ?? "",
                ["extraction_method"] = "Playwright_TicketStation",
                ["crawled_from"] = "https://ticketstation.bg/bg/attractions"
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
                process.WaitForExit(TimeSpan.FromMinutes(5)); // 5 minute timeout

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

    private static bool IsSofiaCity(string city)
    {
        var sofiaCities = new[]
        {
        "софия", "sofia", "софија", "sofija",
        "гр. софия", "гр.софия", "sofia city"
    };

        return sofiaCities.Any(sc => city.Contains(sc));
    }
}
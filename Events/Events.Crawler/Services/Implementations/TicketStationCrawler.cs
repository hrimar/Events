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

            var ticketStationEvents = await GetTicketStationEventsAsync("https://ticketstation.bg/bg/top-events");

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
                    // Load the main page
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

                    // Process cards one by one to avoid stale element references
                    int cardIndex = 0;
                    int totalCards = itemCards.Count;

                    while (cardIndex < totalCards)
                    {
                        try
                        {
                            // Re-query the cards on the main page to get fresh references
                            itemCards = await page.QuerySelectorAllAsync(".item");

                            if (cardIndex >= itemCards.Count)
                            {
                                _logger.LogWarning("Card index {CardIndex} is out of range. Total cards: {TotalCards}", cardIndex, itemCards.Count);
                                break;
                            }

                            var card = itemCards[cardIndex];
                            var eventDto = new TicketStationEventDto();

                            // Step 1: Extract basic info from main page                            
                            // Extract name from .item-name
                            var nameElement = await card.QuerySelectorAsync(".item-name");
                            if (nameElement != null)
                            {
                                eventDto.Name = await nameElement.InnerTextAsync();
                            }

                            // Extract city from .item-city
                            var cityElement = await card.QuerySelectorAsync(".item-city");
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

                            // Get href for detail page (Step 2)
                            var detailHref = await card.GetAttributeAsync("href");
                            if (string.IsNullOrEmpty(detailHref))
                            {
                                _logger.LogWarning("No href found for event: {EventName}", eventDto.Name);
                                cardIndex++;
                                continue;
                            }

                            // Make detail URL absolute
                            var detailUrl = detailHref.StartsWith("/") ? $"https://ticketstation.bg{detailHref}" : detailHref;

                            eventDto.Url = "https://ticketstation.bg";

                            // Step 2: Navigate to detail page to extract price
                            try
                            {
                                await page.GotoAsync(detailUrl, new PageGotoOptions
                                {
                                    WaitUntil = WaitUntilState.DOMContentLoaded,
                                    Timeout = 30000
                                });

                                await Task.Delay(2000); // Wait for content to load

                                // Extract price from .item-price
                                var priceElement = await page.QuerySelectorAsync(".item-price");
                                if (priceElement != null)
                                {
                                    var priceText = await priceElement.InnerTextAsync();
                                    eventDto.Price = ExtractLowestPriceInEur(priceText);
                                }

                                // Find .item element to get href for Step 3 (location/booking page)
                                var itemElement = await page.QuerySelectorAsync(".item");
                                if (itemElement != null)
                                {
                                    var locationHref = await itemElement.GetAttributeAsync("href");
                                    if (!string.IsNullOrEmpty(locationHref))
                                    {
                                        // Make location URL absolute
                                        var locationUrl = locationHref.StartsWith("/") ? $"https://ticketstation.bg{locationHref}" : locationHref;

                                        // Step 3: Navigate to location/booking page
                                        await page.GotoAsync(locationUrl, new PageGotoOptions
                                        {
                                            WaitUntil = WaitUntilState.DOMContentLoaded,
                                            Timeout = 30000
                                        });

                                        await Task.Delay(2000);

                                        // Set TicketUrl to this final page URL
                                        eventDto.TicketUrl = locationUrl;

                                        // Extract description
                                        var descriptionElement = await page.QuerySelectorAsync("#collapseDesc");
                                        if (descriptionElement != null)
                                        {
                                            var descriptionText = await descriptionElement.InnerTextAsync();
                                            eventDto.Description = descriptionText.Replace("Виж още", "").Trim();
                                        }

                                        // Extract location from venue link
                                        var venueLink = await page.QuerySelectorAsync("a.text-wrap");
                                        if (venueLink != null)
                                        {
                                            var venueSpan = await venueLink.QuerySelectorAsync("span");
                                            if (venueSpan != null)
                                            {
                                                var locationText = await venueSpan.InnerTextAsync();

                                                // Parse location: "Арена 8888 София" → venue="Арена 8888"
                                                // Remove city name from location text if present
                                                if (!string.IsNullOrEmpty(eventDto.City))
                                                {
                                                    locationText = locationText.Replace(eventDto.City, "").Trim();
                                                    eventDto.Location = locationText;
                                                }
                                            }
                                        }

                                        // Extract date from description (set to MinValue if not found)
                                        if (!string.IsNullOrEmpty(eventDto.Description))
                                        {
                                            // TODO: Try to parse date from description "на 15 май 2026 г" but for now set to MinValue
                                            eventDto.Date = DateTime.MinValue.ToString("yyyy-MM-dd");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("No href found in .item element for Step 3: {EventName}", eventDto.Name);
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("No .item element found for Step 3: {EventName}", eventDto.Name);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error extracting details for event: {EventName}", eventDto.Name);
                            }

                            // Navigate back to main page for next item
                            await page.GotoAsync(url, new PageGotoOptions
                            {
                                WaitUntil = WaitUntilState.DOMContentLoaded,
                                Timeout = 30000
                            });
                            await Task.Delay(2000);

                            // Only add events that have at least a name
                            if (!string.IsNullOrEmpty(eventDto.Name))
                            {
                                events.Add(eventDto);
                                _logger.LogDebug("Extracted event: {EventName} on {EventDate} in {EventCity}", eventDto.Name, eventDto.Date, eventDto.City);
                            }

                            // Move to next card
                            cardIndex++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error extracting event data from card at index {CardIndex}", cardIndex);

                            // Try to navigate back to main page
                            try
                            {
                                await page.GotoAsync(url, new PageGotoOptions
                                {
                                    WaitUntil = WaitUntilState.DOMContentLoaded,
                                    Timeout = 30000
                                });
                                await Task.Delay(2000);
                            }
                            catch (Exception navEx)
                            {
                                _logger.LogError(navEx, "Failed to navigate back to main page after error");
                            }

                            cardIndex++;
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
            _logger.LogDebug("Filtering out non-Sofia event: {EventName} in {City}", ticketStationEvent.Name, ticketStationEvent.City);
            return null; // Skip non-Sofia events
        }

        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(ticketStationEvent.Name, ticketStationEvent.Url),
            Source = SourceName,
            Name = CleanText(ticketStationEvent.Name) ?? "Unknown Event",
            Description = CleanText(ticketStationEvent.Description),
            StartDate = TryParseEventDate(ticketStationEvent.Date),
            City = CleanText(ticketStationEvent.City) ?? "",
            Location = ticketStationEvent.Location,
            ImageUrl = ticketStationEvent.ImageUrl,
            SourceUrl = ticketStationEvent.Url,
            TicketUrl = ticketStationEvent.TicketUrl,
            Price = ticketStationEvent.Price,
            IsFree = ticketStationEvent.Price == null || ticketStationEvent.Price == 0,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = ticketStationEvent.Date ?? "",
                ["city"] = ticketStationEvent.City ?? "",
                ["extraction_method"] = "Playwright_TicketStation",
                ["crawled_from"] = "https://ticketstation.bg/bg/top-events"
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

    /// <summary>
    /// Extracts the lowest price in EUR from text that may contain multiple prices.
    /// Example: "30 BGN - 50 BGN\n15.34 EUR - 25.56 EUR" → 15.34
    /// </summary>
    private decimal? ExtractLowestPriceInEur(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            // Regex pattern to match decimal numbers before "EUR"
            // Matches: "15.34 EUR", "15,34 EUR", "15 EUR", "25.56EUR"
            // Pattern: optional whitespace, number (with optional . or , decimal separator), optional whitespace, "EUR"
            var pattern = @"(\d+[.,]?\d*)\s*EUR";
            var matches = System.Text.RegularExpressions.Regex.Matches(text, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (matches.Count == 0)
            {
                _logger.LogDebug("No EUR prices found in text: {Text}", text);
                return null;
            }

            var prices = new List<decimal>();

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var priceStr = match.Groups[1].Value;

                // Replace comma with dot for decimal parsing (European format)
                priceStr = priceStr.Replace(',', '.');

                if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                {
                    prices.Add(price);
                    _logger.LogDebug("Found EUR price: {Price}", price);
                }
            }

            if (prices.Count == 0)
            {
                _logger.LogDebug("No valid EUR prices could be parsed from text");
                return null;
            }

            // Return the lowest price
            var lowestPrice = prices.Min();
            _logger.LogDebug("Extracted lowest price: {LowestPrice} EUR from {TotalPrices} prices found", lowestPrice, prices.Count);

            return lowestPrice;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting EUR price from text: {Text}", text);
            return null;
        }
    }
}
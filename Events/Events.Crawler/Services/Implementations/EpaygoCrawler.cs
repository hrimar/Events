using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.Epaygo;
using Events.Crawler.Enums;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class EpaygoCrawler : IWebScrapingCrawler
{
    private readonly ILogger<EpaygoCrawler> _logger;
    private readonly int _maxRetries = 3;
    private readonly int _delayBetweenRequests = 2000;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    public string SourceName => "epaygo.bg";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public EpaygoCrawler(ILogger<EpaygoCrawler> logger)
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

            var epaygoEvents = await GetEpaygoEventsAsync("https://epaygo.bg/events/all");

            var sofiaEvents = epaygoEvents
                .Select(MapEpaygoToStandardDto)
                .Where(e => e != null)
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = epaygoEvents.Count;
            result.Events = sofiaEvents;
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} events from Epaygo, {SofiaEvents} in Sofia",
                epaygoEvents.Count, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling Epaygo.bg");
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

    private async Task<List<EpaygoEventDto>> GetEpaygoEventsAsync(string url)
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
                    await page.GotoAsync(url, new PageGotoOptions
                    {
                        WaitUntil = WaitUntilState.NetworkIdle,
                        Timeout = 60000
                    });

                    // Wait for the event content boxes to load
                    try
                    {
                        await page.WaitForSelectorAsync(".event_content_box", new PageWaitForSelectorOptions { Timeout = 15000 });
                    }
                    catch (TimeoutException)
                    {
                        _logger.LogWarning("Timeout waiting for .event_content_box elements");
                        await Task.Delay(5000); // Fallback wait
                    }

                    // Additional wait for dynamic content
                    await Task.Delay(3000);

                    // Get all elements with class 'event_content_box'
                    var eventElements = await page.QuerySelectorAllAsync(".event_content_box");
                    var events = new List<EpaygoEventDto>();

                    _logger.LogInformation("Found {EventCount} event content boxes", eventElements.Count);

                    foreach (var element in eventElements)
                    {
                        try
                        {
                            var eventDto = new EpaygoEventDto();

                            // Navigate to the col_all_box div within each event_content_box
                            var colAllBox = await element.QuerySelectorAsync(".col_all_box");
                            if (colAllBox == null)
                            {
                                continue;
                            }

                            // Extract image URL from .h_all element's style attribute
                            var imageElement = await colAllBox.QuerySelectorAsync(".h_all");
                            if (imageElement != null)
                            {
                                var styleAttribute = await imageElement.GetAttributeAsync("style");
                                if (!string.IsNullOrEmpty(styleAttribute))
                                {
                                    // Extract URL from background-image: url('...') pattern
                                    var match = Regex.Match(styleAttribute, @"background-image:\s*url\(['""]?([^'""]+)['""]?\)");
                                    if (match.Success)
                                    {
                                        eventDto.ImageUrl = match.Groups[1].Value;
                                        // Make URL absolute if it's relative
                                        if (!string.IsNullOrEmpty(eventDto.ImageUrl) && eventDto.ImageUrl.StartsWith("/"))
                                        {
                                            var baseUri = new Uri(url);
                                            eventDto.ImageUrl = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.ImageUrl}";
                                        }
                                    }
                                }

                                // Extract ticket purchase URL from .event_order_btn within .h_all
                                var orderBtnElement = await imageElement.QuerySelectorAsync(".event_order_btn a[href]");
                                if (orderBtnElement != null)
                                {
                                    eventDto.TicketUrl = await orderBtnElement.GetAttributeAsync("href");
                                    // Make URL absolute if it's relative
                                    if (!string.IsNullOrEmpty(eventDto.TicketUrl) && eventDto.TicketUrl.StartsWith("/"))
                                    {
                                        var baseUri = new Uri(url);
                                        eventDto.TicketUrl = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.TicketUrl}";
                                    }
                                }
                            }

                            // Extract date(s) from .headline_date_t_all element(s) - Handle both single and date ranges
                            var dateElements = await colAllBox.QuerySelectorAllAsync(".headline_date_t_all");
                            if (dateElements.Count > 0)
                            {
                                var dateTexts = new List<string>();
                                foreach (var dateEl in dateElements)
                                {
                                    var dateText = await dateEl.InnerTextAsync();
                                    if (!string.IsNullOrWhiteSpace(dateText))
                                    {
                                        dateTexts.Add(dateText.Trim());
                                    }
                                }

                                // Concatenate all dates (for date ranges or multiple dates)
                                if (dateTexts.Count > 1)
                                {
                                    eventDto.Date = string.Join(" - ", dateTexts); // Join with " - " for date ranges
                                }
                                else if (dateTexts.Count == 1)
                                {
                                    eventDto.Date = dateTexts[0]; // Single date
                                }
                            }

                            // Extract name from .headline_text_t_all element
                            var nameElement = await colAllBox.QuerySelectorAsync(".headline_text_t_all");
                            if (nameElement != null)
                            {
                                eventDto.Name = await nameElement.InnerTextAsync();
                            }

                            // Fallback URL extraction if not found in order button
                            if (string.IsNullOrEmpty(eventDto.TicketUrl))
                            {
                                var linkElement = await element.QuerySelectorAsync("a[href]");
                                if (linkElement != null)
                                {
                                    var href = await linkElement.GetAttributeAsync("href");
                                    if (!string.IsNullOrEmpty(href))
                                    {
                                        // Make URL absolute if it's relative
                                        if (href.StartsWith("/"))
                                        {
                                            var baseUri = new Uri(url);
                                            eventDto.TicketUrl = $"{baseUri.Scheme}://{baseUri.Host}{href}";
                                        }
                                        else
                                        {
                                            eventDto.TicketUrl = href;
                                        }
                                    }
                                }
                            }

                            // Set description as the full text content for now
                            eventDto.Description = await element.InnerTextAsync();

                            // Try to extract price from description if available
                            var fullText = eventDto.Description;
                            if (!string.IsNullOrEmpty(fullText))
                            {
                                // Look for price patterns
                                var priceMatch = Regex.Match(fullText, @"(\d+(?:\.\d{2})?)\s*(?:лв|BGN|EUR)", RegexOptions.IgnoreCase);
                                if (priceMatch.Success && decimal.TryParse(priceMatch.Groups[1].Value, out decimal price))
                                {
                                    eventDto.Price = price;
                                    eventDto.IsFree = price == 0;
                                }
                            }

                            // Set source URL to the current page URL
                            eventDto.SourceUrl = url;

                            // Only add events that have at least a name
                            if (!string.IsNullOrEmpty(eventDto.Name))
                            {
                                events.Add(eventDto);

                                _logger.LogDebug("Extracted event: {EventName} on {EventDate} with price {Price}",
                                    eventDto.Name, eventDto.Date, eventDto.Price);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error extracting event data from Epaygo element");
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
                _logger.LogWarning(ex, "Attempt {Retry} failed for Epaygo crawl", retryCount);

                if (retryCount >= _maxRetries)
                    throw;

                await Task.Delay(_delayBetweenRequests * retryCount);
            }
        }

        return new List<EpaygoEventDto>();
    }

    private CrawledEventDto? MapEpaygoToStandardDto(EpaygoEventDto epaygoEvent)
    {
        // Since all events from Epaygo are Sofia events, no filtering needed
        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(epaygoEvent.Name, epaygoEvent.TicketUrl),
            Source = SourceName,
            Name = CleanText(epaygoEvent.Name) ?? "Unknown Event",
            Description = CleanText(epaygoEvent.Description),
            StartDate = TryParseEventDate(epaygoEvent.Date),
            Location = CleanText(epaygoEvent.City) ?? "София",
            ImageUrl = epaygoEvent.ImageUrl,
            SourceUrl = epaygoEvent.SourceUrl,
            TicketUrl = epaygoEvent.TicketUrl,
            Price = epaygoEvent.Price,
            IsFree = epaygoEvent.IsFree || epaygoEvent.Price == null || epaygoEvent.Price == 0,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = epaygoEvent.Date ?? "",
                ["city"] = epaygoEvent.City ?? "",
                ["extraction_method"] = "Playwright_Epaygo",
                ["crawled_from"] = "https://epaygo.bg/events/all"
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
            "dd/MM/yyyy HH:mm",
            "dd-MM-yyyy",
            "dd-MM-yyyy HH:mm"
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
}
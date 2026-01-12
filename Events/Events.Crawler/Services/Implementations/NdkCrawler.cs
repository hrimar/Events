using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.Ndk;
using Events.Crawler.Enums;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class NdkCrawler : IWebScrapingCrawler
{
    private const string BaseUrl = "https://www.tickets.ndk.bg";
    private const string EventsContainerSelector = "#eventsListHolder";
    private const string EventBoxSelector = ".contentBoxFramed";
    private const string EventEntrySelector = ".eventEntryLong";
    private const string DateSelector = ".eventEntryLongLeftDate";
    private const string NameSelector = ".eventEntryLongRight";
    private const string ImageContainerSelector = ".eventEntryLongLeft";
    private const string ControlsLineSelector = ".controlsLine.controlsLineBlack";
    private const string TimeLocationSelector = ".controlsLineLeft";
    private const string TicketLinkSelector = ".controlsLineRight a[href]";

    private readonly ILogger<NdkCrawler> _logger;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    private static readonly Dictionary<string, int> Months = new(StringComparer.OrdinalIgnoreCase)
    {
        ["яну"] = 1, ["jan"] = 1,
        ["фев"] = 2, ["feb"] = 2,
        ["мар"] = 3, ["mar"] = 3,
        ["апр"] = 4, ["apr"] = 4,
        ["май"] = 5, ["may"] = 5,
        ["юни"] = 6, ["jun"] = 6,
        ["юли"] = 7, ["jul"] = 7,
        ["авг"] = 8, ["aug"] = 8,
        ["сеп"] = 9, ["sep"] = 9,
        ["окт"] = 10, ["oct"] = 10,
        ["ное"] = 11, ["nov"] = 11,
        ["дек"] = 12, ["dec"] = 12
    };

    public string SourceName => "ndk.bg";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public NdkCrawler(ILogger<NdkCrawler> logger)
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

            var ndkEvents = await GetNdkEventsAsync("https://www.tickets.ndk.bg/npc_ws_pg_allevents.php?lang=bgr");

            var sofiaEvents = ndkEvents
                .Select(MapNdkToStandardDto)
                .Where(e => e != null)
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = ndkEvents.Count;
            result.Events = sofiaEvents;
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} events from NDK, {SofiaEvents} in Sofia", ndkEvents.Count, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling NDK.bg");
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
            var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-dev-shm-usage"
                    }
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
            var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[]
                    {
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-dev-shm-usage"
                    }
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

    private async Task<List<NdkEventDto>> GetNdkEventsAsync(string url)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
        });
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });

            try
            {
                await page.WaitForSelectorAsync(EventsContainerSelector, new PageWaitForSelectorOptions { Timeout = 15000 });
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout waiting for {Selector} element", EventsContainerSelector);
                await Task.Delay(3000);
            }

            await Task.Delay(2000);

            var totalElements = await page.EvaluateAsync<int>($"document.querySelectorAll('{EventsContainerSelector} {EventBoxSelector}').length");
            _logger.LogInformation("Found {EventCount} event boxes in NDK", totalElements);

            var events = new List<NdkEventDto>();
            const int batchSize = 25;

            for (int batchStart = 0; batchStart < totalElements; batchStart += batchSize)
            {
                var batchEnd = Math.Min(batchStart + batchSize, totalElements);
                _logger.LogDebug("Processing batch {Start}-{End} of {Total}", batchStart + 1, batchEnd, totalElements);

                var batchElements = await page.QuerySelectorAllAsync($"{EventsContainerSelector} {EventBoxSelector}");
                var elementsToProcess = batchElements.Skip(batchStart).Take(batchEnd - batchStart);

                foreach (var element in elementsToProcess)
                {
                    var eventDto = await ExtractEventDataAsync(element, url);
                    if (eventDto?.Name != null)
                        events.Add(eventDto);
                }

                if (batchStart + batchSize < totalElements)
                    await Task.Delay(100);

                if (batchStart > 0 && batchStart % 100 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    await Task.Delay(200);
                    _logger.LogDebug("Memory cleanup after processing {ProcessedCount} elements", batchStart);
                }
            }

            _logger.LogInformation("Successfully extracted {EventCount} events from NDK", events.Count);
            return events;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task<NdkEventDto?> ExtractEventDataAsync(IElementHandle element, string sourceUrl)
    {
        var eventEntryLong = await element.QuerySelectorAsync(EventEntrySelector);
        if (eventEntryLong == null)
            return null;

        var eventDto = new NdkEventDto { SourceUrl = sourceUrl };

        eventDto.Date = await ExtractDateAsync(eventEntryLong);
        eventDto.Name = await ExtractNameAsync(eventEntryLong);
        eventDto.ImageUrl = await ExtractImageUrlAsync(eventEntryLong);

        var timeAndLocation = await ExtractTimeAndLocationAsync(element);
        eventDto.Time = timeAndLocation.Time;
        eventDto.Location = timeAndLocation.Location;

        eventDto.TicketUrl = await ExtractTicketUrlAsync(element);
        eventDto.IsFree = false;

        if (!string.IsNullOrEmpty(eventDto.Name))
        {
            _logger.LogDebug("Extracted event: {EventName} on {EventDate} at {EventTime}", eventDto.Name, eventDto.Date, eventDto.Time);
        }

        return eventDto;
    }

    private async Task<string?> ExtractDateAsync(IElementHandle element)
    {
        try
        {
            var dateElement = await element.QuerySelectorAsync(DateSelector);
            if (dateElement == null)
                return null;

            var dateText = await dateElement.InnerTextAsync();
            return string.IsNullOrWhiteSpace(dateText) ? null : CleanText(dateText);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error extracting date: {Error}", ex.Message);
            return null;
        }
    }

    private async Task<string?> ExtractNameAsync(IElementHandle element)
    {
        try
        {
            var nameElement = await element.QuerySelectorAsync(NameSelector);
            if (nameElement == null)
                return null;

            return await nameElement.InnerTextAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error extracting name: {Error}", ex.Message);
            return null;
        }
    }

    private async Task<string?> ExtractImageUrlAsync(IElementHandle element)
    {
        try
        {
            var imageElement = await element.QuerySelectorAsync(ImageContainerSelector);
            if (imageElement == null)
                return null;

            var styleAttr = await imageElement.GetAttributeAsync("style");
            if (string.IsNullOrEmpty(styleAttr))
                return null;

            var match = Regex.Match(styleAttr, @"background-image:\s*url\((?<url>[^)]+)\)");
            if (!match.Success)
                return null;

            var imagePath = match.Groups["url"].Value.Trim(new[] { '"', '\'' });
            return $"{BaseUrl}/{imagePath}";
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error extracting image URL: {Error}", ex.Message);
            return null;
        }
    }

    private async Task<(string? Time, string? Location)> ExtractTimeAndLocationAsync(IElementHandle element)
    {
        try
        {
            var timeElements = await element.QuerySelectorAllAsync(TimeLocationSelector);
            if (timeElements.Count < 4)
                return (null, null);

            var timeText = await timeElements[1].InnerTextAsync();
            var time = string.IsNullOrWhiteSpace(timeText) ? null : timeText.Trim();

            var locationText = await timeElements[3].InnerTextAsync();
            var location = string.IsNullOrWhiteSpace(locationText) ? null : $"НДК - {locationText}";

            return (time, location);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error extracting time and location: {Error}", ex.Message);
            return (null, null);
        }
    }

    private async Task<string?> ExtractTicketUrlAsync(IElementHandle element)
    {
        try
        {
            var controlsLine = await element.QuerySelectorAsync(ControlsLineSelector);
            if (controlsLine == null)
                return null;

            var ticketLink = await controlsLine.QuerySelectorAsync(TicketLinkSelector);
            if (ticketLink == null)
                return null;

            var href = await ticketLink.GetAttributeAsync("href");
            if (string.IsNullOrEmpty(href))
                return null;

            return href.StartsWith("/") ? $"{BaseUrl}{href}" : href;
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error extracting ticket URL: {Error}", ex.Message);
            return null;
        }
    }

    private CrawledEventDto? MapNdkToStandardDto(NdkEventDto ndkEvent)
    {
        // All NDK events are in Sofia
        var r = new CrawledEventDto
        {
            ExternalId = GenerateEventId(ndkEvent.Name, ndkEvent.TicketUrl),
            Source = SourceName,
            Name = CleanText(ndkEvent.Name) ?? "",
            Description = null,
            StartDate = TryParseEventDateTime(ndkEvent.Date, ndkEvent.Time),
            City = "София",
            Location = CleanText(ndkEvent.Location) ?? "НДК",
            ImageUrl = ndkEvent.ImageUrl,
            SourceUrl = ndkEvent.SourceUrl,
            TicketUrl = ndkEvent.TicketUrl,
            IsFree = ndkEvent.IsFree,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = ndkEvent.Date ?? "",
                ["original_time_text"] = ndkEvent.Time ?? "",
                ["location"] = ndkEvent.Location ?? "",
                ["extraction_method"] = "Playwright_NDK_EventLists",
                ["crawled_from"] = "https://www.tickets.ndk.bg/npc_ws_pg_allevents.php"
            }
        };
        return r;
    }

    private DateTime? TryParseEventDateTime(string? dateText, string? timeText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
            return null;

        var date = ParseNdkDate(dateText);
        if (!date.HasValue)
            return null;

        if (string.IsNullOrWhiteSpace(timeText))
            return date.Value;

        // Try to parse time and combine with date
        try
        {
            var timeMatch = Regex.Match(timeText.Trim(), @"(\d{1,2}):(\d{2})");
            if (timeMatch.Success && int.TryParse(timeMatch.Groups[1].Value, out int hour) && int.TryParse(timeMatch.Groups[2].Value, out int minute))
            {
                return date.Value.AddHours(hour).AddMinutes(minute);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Error parsing time {TimeText}: {Error}", timeText, ex.Message);
        }

        return date.Value;
    }

    private DateTime? ParseNdkDate(string dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
            return null;

        try
        {
            var normalized = dateText.Replace("\n", " ").Replace("\r", "").Trim();
            normalized = Regex.Replace(normalized, @"\s+", " ");

            // Format: "20 APR 2026" or "20 АПР 2026"
            var pattern = @"(\b\d{1,2})\s+([a-zA-Zа-яА-Я]+)\s+(\d{4}\b)";
            var match = Regex.Match(normalized, pattern);

            if (!match.Success)
                return null;

            var dayStr = match.Groups[1].Value;
            var monthStr = match.Groups[2].Value.ToLowerInvariant();
            var yearStr = match.Groups[3].Value;

            if (!int.TryParse(dayStr, out int day) || day < 1 || day > 31)
                return null;

            if (!int.TryParse(yearStr, out int year))
                return null;

            if (!Months.TryGetValue(monthStr, out int month))
                return null;

            if (!IsValidDate(year, month, day))
                return null;

            var parsedDate = new DateTime(year, month, day);
            _logger.LogDebug("Parsed NDK date '{DateText}' as {ParsedDate}", dateText, parsedDate.ToString("yyyy-MM-dd"));

            return parsedDate;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error parsing NDK date: {DateText}", dateText);
            return null;
        }
    }

    private bool IsValidDate(int year, int month, int day)
    {
        try
        {
            _ = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GenerateEventId(string? title, string? url)
    {
        var combined = $"{title}|{url ?? ""}";
        return Math.Abs(combined.GetHashCode()).ToString();
    }

    private string? CleanText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        return text.Trim().Replace("\n", " ").Replace("\r", "").Replace("  ", " ");
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
                throw new InvalidOperationException("Playwright browsers are not installed. " + "Please run 'npx playwright install chromium' manually.", ex);
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

        if (!Directory.Exists(playwrightDir))
            return null;

        var chromiumDirs = Directory.GetDirectories(playwrightDir, "chromium-*");
        if (!chromiumDirs.Any())
            return null;

        var latestChromiumDir = chromiumDirs.OrderByDescending(d => d).First();
        var chromePath = Path.Combine(latestChromiumDir, "chrome-win", "chrome.exe");

        return File.Exists(chromePath) ? chromePath : null;
    }
}

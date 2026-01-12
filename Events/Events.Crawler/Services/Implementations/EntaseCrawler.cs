using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.Entase;
using Events.Crawler.Enums;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Events.Crawler.Services.Implementations;

public class EntaseCrawler : IWebScrapingCrawler
{
    private const string BaseUrl = "https://www.entase.com";
    private const string EventsListUrl = "https://www.entase.com/?city=София";
    private const string EventCardSelector = ".productions_grid_item_content";
    private const string ImageContainerSelector = ".productions_grid_item_cover";
    private const string ImageSelector = "img";
    private const string EventNameSelector = ".productions_grid_item_title.line-2";
    private const string ProductionDescriptionSelector = "._productiondsc";
    private const string PresentationSelector = "._presentation";
    private const string SpectacleSelector = ".row.events_grid_item.onetime";
    private const string DateSelector = ".eg_item_date";
    private const string LocationSelector = ".eg_item_titlelocation";
    private const string TicketButtonSelector = ".eg_item_button a";

    private readonly ILogger<EntaseCrawler> _logger;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    public string SourceName => "entase.com";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public EntaseCrawler(ILogger<EntaseCrawler> logger)
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

            var entaseEvents = await GetEntaseEventsAsync();

            var sofiaEvents = entaseEvents
                .SelectMany(e => e.Spectacles.Select(s => MapToStandardDto(e, s)))
                .Where(e => e != null)
                .Cast<CrawledEventDto>()
                .ToList();

            result.EventsFound = entaseEvents.Sum(e => e.Spectacles.Count);
            result.Events = sofiaEvents;
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("Crawled {TotalEvents} spectacles from Entase ({EventCount} events with spectacles)", sofiaEvents.Count, entaseEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crawling Entase.com");
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
                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
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
                await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
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

    private async Task<List<EntaseEventDto>> GetEntaseEventsAsync()
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
            await page.GotoAsync(EventsListUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });
            await Task.Delay(2000);

            // Scroll to load all events
            await ScrollToLoadAllEventsAsync(page);

            // Extract card data (name, image, link) from list page first
            var cardDataList = await ExtractCardDataAsync(page);
            _logger.LogInformation("Found {EventCount} event cards on Entase", cardDataList.Count);

            var events = new List<EntaseEventDto>();

            foreach (var cardData in cardDataList)
            {
                try
                {
                    var eventDto = await ExtractEventDetailsAsync(page, cardData);
                    if (eventDto != null && eventDto.Spectacles.Any())
                    {
                        events.Add(eventDto);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error extracting event details for {EventName}", cardData.Name);
                }
            }

            _logger.LogInformation("Successfully extracted {EventCount} events with spectacles from Entase", events.Count);
            return events;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task<List<(string Name, string ImageUrl, string DetailUrl)>> ExtractCardDataAsync(IPage page)
    {
        var cardDataList = new List<(string, string, string)>();
        try
        {
            var eventCards = await page.QuerySelectorAllAsync(EventCardSelector);

            foreach (var eventCard in eventCards)
            {
                try
                {
                    var name = await ExtractEventNameAsync(eventCard);
                    if (string.IsNullOrEmpty(name))
                        continue;

                    var imageUrl = await ExtractImageUrlAsync(eventCard);
                    var cardLink = await eventCard.QuerySelectorAsync("a");
                    if (cardLink == null)
                        continue;

                    var href = await cardLink.GetAttributeAsync("href");
                    if (string.IsNullOrEmpty(href))
                        continue;

                    var detailsUrl = href.StartsWith("http") ? href : $"{BaseUrl}{href}";
                    cardDataList.Add((name, imageUrl ?? "", detailsUrl));
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error extracting card data");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error in ExtractCardDataAsync");
        }

        return cardDataList;
    }

    private async Task ScrollToLoadAllEventsAsync(IPage page)
    {
        var previousHeight = 0;
        var maxScrollAttempts = 50;
        var scrollAttempts = 0;

        while (scrollAttempts < maxScrollAttempts)
        {
            var currentHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");

            if (currentHeight == previousHeight)
            {
                _logger.LogDebug("Reached end of page after {Attempts} scroll attempts", scrollAttempts);
                break;
            }

            previousHeight = currentHeight;
            await page.EvaluateAsync("window.scrollBy(0, window.innerHeight)");
            await Task.Delay(500);
            scrollAttempts++;
        }
    }

    private async Task<EntaseEventDto?> ExtractEventDetailsAsync(IPage page, (string Name, string ImageUrl, string DetailUrl) cardData)
    {
        var eventDto = new EntaseEventDto
        {
            Name = cardData.Name,
            ImageUrl = cardData.ImageUrl,
            SourceUrl = BaseUrl
        };

        // Navigate to details page to get description and spectacles
        try
        {
            await page.GotoAsync(cardData.DetailUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30000 });
            await Task.Delay(1000);

            // Extract description
            eventDto.Description = await ExtractDescriptionAsync(page);

            // Extract spectacles
            var spectacles = await ExtractSpectaclesAsync(page);
            eventDto.Spectacles = spectacles;

            _logger.LogDebug("Extracted event '{EventName}' with {SpectacleCount} spectacles", eventDto.Name, spectacles.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting details for event '{EventName}'", eventDto.Name);
            return null;
        }

        return eventDto;
    }

    private async Task<string?> ExtractEventNameAsync(IElementHandle eventCard)
    {
        try
        {
            var nameElement = await eventCard.QuerySelectorAsync(EventNameSelector);
            if (nameElement == null)
                return null;

            var name = await nameElement.InnerTextAsync();
            return string.IsNullOrWhiteSpace(name) ? null : CleanText(name);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting event name");
            return null;
        }
    }

    private async Task<string?> ExtractImageUrlAsync(IElementHandle eventCard)
    {
        try
        {
            var imageContainer = await eventCard.QuerySelectorAsync(ImageContainerSelector);
            if (imageContainer == null)
                return null;

            var imgElement = await imageContainer.QuerySelectorAsync(ImageSelector);
            if (imgElement == null)
                return null;

            var src = await imgElement.GetAttributeAsync("src");
            if (string.IsNullOrEmpty(src))
                return null;

            return src.StartsWith("http") ? src : $"{BaseUrl}{src}";
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting image URL");
            return null;
        }
    }

    private async Task<string?> ExtractDescriptionAsync(IPage page)
    {
        try
        {
            // First find the ._productiondsc container
            var productionDescContainer = await page.QuerySelectorAsync(ProductionDescriptionSelector);
            if (productionDescContainer == null)
                return null;

            // Then find ._presentation inside it
            var presentationElement = await productionDescContainer.QuerySelectorAsync(PresentationSelector);
            if (presentationElement == null)
                return null;

            var description = await presentationElement.InnerTextAsync();
            return string.IsNullOrWhiteSpace(description) ? null : CleanText(description.Replace("#", "").Replace("@", "").Trim());
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting description");
            return null;
        }
    }

    private async Task<List<EntaseSpectacleDto>> ExtractSpectaclesAsync(IPage page)
    {
        var spectacles = new List<EntaseSpectacleDto>();

        try
        {
            var spectacleElements = await page.QuerySelectorAllAsync(SpectacleSelector);

            foreach (var spectacleElement in spectacleElements)
            {
                try
                {
                    var spectacle = new EntaseSpectacleDto();

                    // Extract date
                    spectacle.DateText = await ExtractSpectacleDateAsync(spectacleElement);

                    // Extract location
                    spectacle.Location = await ExtractSpectacleLocationAsync(spectacleElement);

                    // Extract ticket URL
                    spectacle.TicketUrl = await ExtractSpectacleTicketUrlAsync(spectacleElement);

                    if (!string.IsNullOrEmpty(spectacle.DateText) || !string.IsNullOrEmpty(spectacle.Location))
                    {
                        spectacles.Add(spectacle);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Error extracting spectacle details");
                }
            }

            return spectacles;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting spectacles");
            return spectacles;
        }
    }

    private async Task<string?> ExtractSpectacleDateAsync(IElementHandle spectacleElement)
    {
        try
        {
            var dateElement = await spectacleElement.QuerySelectorAsync(DateSelector);
            if (dateElement == null)
                return null;

            var dateText = await dateElement.InnerTextAsync();
            return string.IsNullOrWhiteSpace(dateText) ? null : NormalizeDateText(dateText);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting spectacle date");
            return null;
        }
    }

    private async Task<string?> ExtractSpectacleLocationAsync(IElementHandle spectacleElement)
    {
        try
        {
            var locationElement = await spectacleElement.QuerySelectorAsync(LocationSelector);
            if (locationElement == null)
                return null;

            var location = await locationElement.InnerTextAsync();
            return string.IsNullOrWhiteSpace(location) ? null : CleanText(location.Replace("place", "").Trim());
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting spectacle location");
            return null;
        }
    }

    private async Task<string?> ExtractSpectacleTicketUrlAsync(IElementHandle spectacleElement)
    {
        try
        {
            var ticketButton = await spectacleElement.QuerySelectorAsync(TicketButtonSelector);
            if (ticketButton == null)
                return null;

            var eventId = await ticketButton.GetAttributeAsync("data-event");
            if (string.IsNullOrEmpty(eventId))
                return null;

            return $"{BaseUrl}/book?eid={eventId}";
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error extracting ticket URL");
            return null;
        }
    }

    private string NormalizeDateText(string dateText)
    {
        // Normalize date format: "08 февруари неделя, 11:00 ч." -> standardized format
        var normalized = CleanText(dateText);
        return Regex.Replace(normalized ?? "", @"\s+", " ");
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
                throw new InvalidOperationException("Playwright browsers are not installed. Please run 'npx playwright install chromium' manually.", ex);
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

    private CrawledEventDto? MapToStandardDto(EntaseEventDto entaseEvent, EntaseSpectacleDto spectacle)
    {
        // Even we get Events from Sofia only, we ignore spectacles witch take place out of Sofia
        if (spectacle.Location != null && !spectacle.Location.Contains("София"))
            return null;

        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(entaseEvent.Name, spectacle.TicketUrl),
            Source = SourceName,
            Name = entaseEvent.Name ?? "",
            Description = entaseEvent.Description,
            StartDate = ParseEntaseDate(spectacle.DateText),
            City = "София",
            Location = spectacle.Location ?? "",
            ImageUrl = entaseEvent.ImageUrl,
            SourceUrl = entaseEvent.SourceUrl,
            TicketUrl = spectacle.TicketUrl,
            IsFree = false,
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = spectacle.DateText ?? "",
                ["location"] = spectacle.Location ?? "",
                ["extraction_method"] = "Playwright_Entase_EventDetails",
                ["crawled_from"] = EventsListUrl
            }
        };
    }

    private DateTime? ParseEntaseDate(string? dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
            return null;

        try
        {
            // Format: "08 февруари неделя, 11:00 ч."
            // Extract date and time components
            var dateMatch = Regex.Match(dateText, @"(\d{1,2})\s+([а-яА-Я]+).*?(\d{1,2}):(\d{2})");
            if (!dateMatch.Success)
                return null;

            var day = int.Parse(dateMatch.Groups[1].Value);
            var monthText = dateMatch.Groups[2].Value.ToLowerInvariant();
            var hour = int.Parse(dateMatch.Groups[3].Value);
            var minute = int.Parse(dateMatch.Groups[4].Value);

            var months = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["януари"] = 1, ["фев"] = 2, ["февруари"] = 2, ["март"] = 3, ["мар"] = 3,
                ["април"] = 4, ["апр"] = 4, ["май"] = 5, ["юни"] = 6, ["юн"] = 6,
                ["юли"] = 7, ["юл"] = 7, ["август"] = 8, ["авг"] = 8, ["септември"] = 9,
                ["сеп"] = 9, ["октомври"] = 10, ["окт"] = 10, ["ноември"] = 11, ["ное"] = 11,
                ["декември"] = 12, ["дек"] = 12
            };

            if (!months.TryGetValue(monthText, out int month))
                return null;

            var year = DateTime.Now.Year;
            if (month < DateTime.Now.Month)
                year++;

            var date = new DateTime(year, month, day, hour, minute, 0);
            return date;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error parsing Entase date: {DateText}", dateText);
            return null;
        }
    }

    private string GenerateEventId(string? title, string? url)
    {
        var combined = $"{title}|{url ?? ""}";
        return Math.Abs(combined.GetHashCode()).ToString();
    }
}

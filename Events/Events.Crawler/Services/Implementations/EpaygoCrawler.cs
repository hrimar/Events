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
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    // Bulgarian months dictionary - created once, reused for all date parsing
    private static readonly Dictionary<string, int> BulgarianMonths = new(StringComparer.OrdinalIgnoreCase)
    {
        ["януари"] = 1, ["ян"] = 1,
        ["февруари"] = 2, ["фев"] = 2,
        ["март"] = 3, ["мар"] = 3,
        ["април"] = 4, ["апр"] = 4,
        ["май"] = 5,
        ["юни"] = 6, ["юн"] = 6,
        ["юли"] = 7, ["юл"] = 7,
        ["август"] = 8, ["авг"] = 8,
        ["септември"] = 9, ["сеп"] = 9,
        ["октомври"] = 10, ["окт"] = 10,
        ["ноември"] = 11, ["ное"] = 11,
        ["декември"] = 12, ["дек"] = 12
    };

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

            _logger.LogInformation("Crawled {TotalEvents} events from Epaygo, {SofiaEvents} in Sofia", epaygoEvents.Count, sofiaEvents.Count);
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
        // Remove nested retry - let CrawlerService handle retries at higher level
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
        });
        var page = await browser.NewPageAsync();

        try
        {
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded, // Faster than NetworkIdle
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
                await Task.Delay(3000); // Reduced from 5000
            }

            await Task.Delay(2000); // Reduced from 3000

            // Process elements in smaller batch size
            var totalElements = await page.EvaluateAsync<int>("document.querySelectorAll('.event_content_box').length");
            _logger.LogInformation("Found {EventCount} event content boxes", totalElements);

            var events = new List<EpaygoEventDto>();
            const int batchSize = 30; // Increased from 20 for better balance

            for (int batchStart = 0; batchStart < totalElements; batchStart += batchSize)
            {
                var batchEnd = Math.Min(batchStart + batchSize, totalElements);
                _logger.LogDebug("Processing batch {Start}-{End} of {Total}", batchStart + 1, batchEnd, totalElements);

                // Get fresh elements for each batch
                var batchElements = await page.QuerySelectorAllAsync($".event_content_box:nth-child(n+{batchStart + 1}):nth-child(-n+{batchEnd})");

                foreach (var element in batchElements.Take(batchEnd - batchStart))
                {
                    try
                    {
                        var eventDto = new EpaygoEventDto();

                        // Navigate to the col_all_box div within each event_content_box
                        var colAllBox = await element.QuerySelectorAsync(".col_all_box");
                        if (colAllBox == null) continue;

                        // Extract image URL from .h_all element's style attribute
                        var imageElement = await colAllBox.QuerySelectorAsync(".h_all");
                        if (imageElement != null)
                        {
                            try
                            {
                                var styleAttribute = await imageElement.GetAttributeAsync("style");
                                if (!string.IsNullOrEmpty(styleAttribute))
                                {
                                    var match = Regex.Match(styleAttribute, @"background-image:\s*url\(['""]?([^'""]+)['""]?\)");
                                    if (match.Success)
                                    {
                                        eventDto.ImageUrl = match.Groups[1].Value;
                                        if (!string.IsNullOrEmpty(eventDto.ImageUrl) && eventDto.ImageUrl.StartsWith("/"))
                                        {
                                            var baseUri = new Uri(url);
                                            eventDto.ImageUrl = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.ImageUrl}";
                                        }
                                    }
                                }

                                var orderBtnElement = await imageElement.QuerySelectorAsync(".event_order_btn a[href]");
                                if (orderBtnElement != null)
                                {
                                    eventDto.TicketUrl = await orderBtnElement.GetAttributeAsync("href");
                                    if (!string.IsNullOrEmpty(eventDto.TicketUrl) && eventDto.TicketUrl.StartsWith("/"))
                                    {
                                        var baseUri = new Uri(url);
                                        eventDto.TicketUrl = $"{baseUri.Scheme}://{baseUri.Host}{eventDto.TicketUrl}";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogDebug("Error extracting image/ticket URL: {Error}", ex.Message);
                            }
                        }

                        // Extract date(s) from .headline_date_t_all element(s)
                        try
                        {
                            var dateElements = await colAllBox.QuerySelectorAllAsync(".headline_date_t_all");
                            if (dateElements.Count > 0)
                            {
                                var dateTexts = new List<string>();
                                foreach (var dateEl in dateElements)
                                {
                                    try
                                    {
                                        var dateText = await dateEl.InnerTextAsync();
                                        if (!string.IsNullOrWhiteSpace(dateText))
                                        {
                                            dateTexts.Add(dateText.Trim());
                                        }
                                    }
                                    catch
                                    {
                                        // Skip problematic date elements
                                    }
                                }

                                if (dateTexts.Count > 1)
                                {
                                    eventDto.Date = string.Join(" - ", dateTexts); // incoming format: "от 06\nфевруари - до 06\nянуари"
                                }
                                else if (dateTexts.Count == 1)
                                {
                                    eventDto.Date = dateTexts[0]; // incoming format: "29\nноември"
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug("Error extracting dates: {Error}", ex.Message);
                        }

                        // Extract name from .headline_text_t_all element
                        try
                        {
                            var nameElement = await colAllBox.QuerySelectorAsync(".headline_text_t_all");
                            if (nameElement != null)
                            {
                                eventDto.Name = await nameElement.InnerTextAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug("Error extracting name: {Error}", ex.Message);
                        }

                        // Fallback URL extraction if not found in order button
                        if (string.IsNullOrEmpty(eventDto.TicketUrl))
                        {
                            try
                            {
                                var linkElement = await element.QuerySelectorAsync("a[href]");
                                if (linkElement != null)
                                {
                                    var href = await linkElement.GetAttributeAsync("href");
                                    if (!string.IsNullOrEmpty(href))
                                    {
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
                            catch (Exception ex)
                            {
                                _logger.LogDebug("Error extracting fallback URL: {Error}", ex.Message);
                            }
                        }

                        // Set description as the full text content for now
                        try
                        {
                            var textContent = await element.InnerTextAsync();
                            eventDto.Description = textContent.Replace("Купи билет", "");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug("Error extracting description: {Error}", ex.Message);
                            eventDto.Description = eventDto.Name; // Fallback
                        }

                        //// TODO: The price is not available in the description. Ignore the price for now
                        //var fullText = eventDto.Description;
                        //if (!string.IsNullOrEmpty(fullText))
                        //{
                        //    var priceMatch = Regex.Match(fullText, @"(\d+(?:\.\d{2})?)\s*(?:лв|BGN|EUR)", RegexOptions.IgnoreCase);
                        //    if (priceMatch.Success && decimal.TryParse(priceMatch.Groups[1].Value, out decimal price))
                        //    {
                        //        eventDto.Price = price;
                        //        eventDto.IsFree = price == 0;
                        //    }
                        //}

                        eventDto.SourceUrl = url;

                        // Only add events that have at least a name
                        if (!string.IsNullOrEmpty(eventDto.Name))
                        {
                            events.Add(eventDto);
                            _logger.LogDebug("Extracted event: {EventName} on {EventDate}", eventDto.Name, eventDto.Date);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Error extracting event data from element");
                    }
                }

                // Brief pause between batches - reduced
                if (batchStart + batchSize < totalElements)
                {
                    await Task.Delay(100); // Reduced from longer delays
                }

                // Memory cleanup between batches
                if (batchStart > 0 && batchStart % 100 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    await Task.Delay(200); // Brief pause
                    _logger.LogDebug("Memory cleanup after processing {ProcessedCount} elements", batchStart);
                }
            }

            _logger.LogInformation("Phase 1 complete: Found {EventCount} events with basic data", events.Count);

            // Phase 2: Extract city and location info - with name-based pre-filtering
            var eventsNeedingDetails = events.Where(e => !string.IsNullOrEmpty(e.TicketUrl)).ToList();
            _logger.LogInformation("Phase 2: Pre-filtering {EventCount} events before city/location extraction", eventsNeedingDetails.Count);

            // Smart name-based pre-filtering: Skip events with other city names in the title
            var likelySofiaEvents = eventsNeedingDetails.Where(e => !IsObviouslyNonSofiaEvent(e)).ToList();
            var skippedNonSofiaCount = eventsNeedingDetails.Count - likelySofiaEvents.Count;
            
            if (skippedNonSofiaCount > 0)
            {
                _logger.LogInformation("Pre-filtered out {SkippedCount} events based on city names in titles", skippedNonSofiaCount);
            }

            _logger.LogInformation("Phase 2: Processing {EventCount} events for city/location data", likelySofiaEvents.Count);

            const int detailBatchSize = 20; // Increased batch size for efficiency
            var processedDetailCount = 0;

            for (int batchStart = 0; batchStart < likelySofiaEvents.Count; batchStart += detailBatchSize)
            {
                var batchEnd = Math.Min(batchStart + detailBatchSize, likelySofiaEvents.Count);
                _logger.LogDebug("Processing detail batch {Start}-{End} of {Total}", batchStart + 1, batchEnd, likelySofiaEvents.Count);

                for (int i = batchStart; i < batchEnd; i++)
                {
                    var eventDto = likelySofiaEvents[i];

                    try
                    {
                        // Fix null reference warning
                        if (string.IsNullOrEmpty(eventDto.TicketUrl))
                        {
                            _logger.LogWarning("Event {EventName} has null/empty TicketUrl, skipping detail extraction", eventDto.Name);
                            continue;
                        }

                        await page.GotoAsync(eventDto.TicketUrl, new PageGotoOptions
                        {
                            WaitUntil = WaitUntilState.DOMContentLoaded,
                            Timeout = 10000 // Further reduced timeout for speed
                        });

                        await Task.Delay(400); // Minimal delay

                        // Extract city from #address element
                        var addressElement = await page.QuerySelectorAsync("#address");
                        if (addressElement != null)
                        {
                            var addressText = await addressElement.InnerTextAsync();
                            if (!string.IsNullOrWhiteSpace(addressText))
                            {
                                // Check if address contains Sofia
                                if (addressText.ToLowerInvariant().Contains("софия") || addressText.ToLowerInvariant().Contains("sofia"))
                                {
                                    eventDto.City = "София";
                                }
                                else
                                {
                                    eventDto.City = null;
                                    _logger.LogDebug("Non-Sofia event: {EventName} at {Address}", eventDto.Name, addressText);
                                }
                            }
                        }

                        // Extract location from #address_t element
                        var locationElement = await page.QuerySelectorAsync("#address_t");
                        if (locationElement != null)
                        {
                            var locationText = await locationElement.InnerTextAsync();
                            if (!string.IsNullOrWhiteSpace(locationText))
                            {
                                eventDto.Location = locationText.Trim();
                            }
                        }

                        processedDetailCount++;
                        _logger.LogDebug("Processed event {Count}/{Total}: {EventName} in {City}", 
                            processedDetailCount, likelySofiaEvents.Count, eventDto.Name, eventDto.City ?? "Unknown");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error extracting details for event: {EventName} at {TicketUrl}", eventDto.Name, eventDto.TicketUrl);
                        // Still keep the event - will be filtered later if needed
                        processedDetailCount++;
                    }
                }

                // Minimal pause between detail batches
                if (batchStart + detailBatchSize < likelySofiaEvents.Count)
                {
                    await Task.Delay(200); // Minimal pause for server kindness
                }

                // Memory cleanup every 60 events
                if (processedDetailCount > 0 && processedDetailCount % 60 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _logger.LogDebug("Memory cleanup after processing {ProcessedCount} detail events", processedDetailCount);
                }
            }

            // Set remaining events that weren't processed as non-Sofia (they were pre-filtered)
            var nonProcessedEvents = eventsNeedingDetails.Except(likelySofiaEvents).ToList();
            foreach (var eventDto in nonProcessedEvents)
            {
                eventDto.City = null; // Will be filtered out in MapEpaygoToStandardDto
            }

            _logger.LogInformation("Successfully processed {EventCount} events from Epaygo ({ProcessedDetails} with full details, {PreFiltered} pre-filtered)", 
                events.Count, processedDetailCount, nonProcessedEvents.Count);
            return events;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private CrawledEventDto? MapEpaygoToStandardDto(EpaygoEventDto epaygoEvent)
    {
        // Filter only Sofia events - early return if not Sofia
        if (string.IsNullOrEmpty(epaygoEvent.City) || 
            (!epaygoEvent.City.ToLowerInvariant().Contains("софия") && 
             !epaygoEvent.City.ToLowerInvariant().Contains("sofia")))
        {
            _logger.LogDebug("Filtering out non-Sofia event: {EventName} in {City}", epaygoEvent.Name, epaygoEvent.City ?? "Unknown");
            return null; // Skip non-Sofia events
        }

        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(epaygoEvent.Name, epaygoEvent.TicketUrl),
            Source = SourceName,
            Name = CleanText(epaygoEvent.Name) ?? "",
            Description = CleanText(epaygoEvent.Description),
            StartDate = TryParseEventDate(epaygoEvent.Date),
            City = CleanText(epaygoEvent.City) ?? "",
            Location = CleanText(epaygoEvent.Location) ?? "",
            ImageUrl = epaygoEvent.ImageUrl,
            SourceUrl = epaygoEvent.SourceUrl,
            TicketUrl = epaygoEvent.TicketUrl,
            //Price = epaygoEvent.Price,
            //IsFree = epaygoEvent.IsFree || epaygoEvent.Price == null || epaygoEvent.Price == 0,
            IsFree = false, // by default all events are considered paid. The admin will adjust if needed the free ones
            RawData = new Dictionary<string, object>
            {
                ["original_date_text"] = epaygoEvent.Date ?? "",
                ["city"] = epaygoEvent.City ?? "",
                ["location"] = epaygoEvent.Location ?? "",
                ["extraction_method"] = "Playwright_Epaygo_OptimizedTiming",
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
        if (string.IsNullOrWhiteSpace(dateText)) return DateTime.MinValue;

        // Attempt to parse Bulgarian format
        var bulgarianDate = ParseBulgarianDate(dateText);
        if (bulgarianDate.HasValue)
        {
            return bulgarianDate.Value;
        }

        // Fallback to common date formats
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
        return DateTime.MinValue; // 0001-01-01 for invalid date
    }

    private DateTime? ParseBulgarianDate(string? dateText)
    {
        if (string.IsNullOrWhiteSpace(dateText))
        {
            return null;
        }

        try
        {
            // Text normalization - remove \n, extra spaces
            var normalized = dateText.Replace("\n", " ").Replace("\r", "").Trim();
            normalized = Regex.Replace(normalized, @"\s+", " "); // Multiple spaces -> one

            var today = DateTime.Today;

            // Check for range: "от 06 февруари до 06 януари" или "06 февруари - 10 март"
            if (normalized.Contains(" до ") || normalized.Contains(" - "))
            {
                var separator = normalized.Contains(" до ") ? " до " : " - ";
                var parts = normalized.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    // Remove "от" if present
                    var startDateText = parts[0].Replace("от", "").Replace("-", "").Trim();
                    var endDateText = parts[1].Trim();

                    var startDate = ParseSingleBulgarianDate(startDateText, today);
                    var endDate = ParseSingleBulgarianDate(endDateText, today);

                    // Take the earlier date, but not before today
                    if (startDate.HasValue && endDate.HasValue)
                    {
                        var earlierDate = startDate.Value < endDate.Value ? startDate.Value : endDate.Value;
                        return earlierDate >= today ? earlierDate : today;
                    }

                    // If only one is valid
                    if (startDate.HasValue)
                    {
                        return startDate.Value >= today ? startDate.Value : today;
                    }

                    if (endDate.HasValue)
                    {
                        return endDate.Value >= today ? endDate.Value : today;
                    }
                }
            }

            // Single date: "29 ноември" или "29 ноември 2025"
            return ParseSingleBulgarianDate(normalized, today);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error parsing Bulgarian date: {DateText}", dateText);
            return null;
        }
    }

    private DateTime? ParseSingleBulgarianDate(string dateText, DateTime today)
    {
        if (string.IsNullOrWhiteSpace(dateText))
        {
            return null;
        }

        try
        {
            // Regex pattern: "29 ноември" or "29 ноември 2025"
            var pattern = @"(\d{1,2})\s+([а-яА-Я]+)(?:\s+(\d{4}))?";
            var match = Regex.Match(dateText, pattern);

            if (!match.Success)
            {
                return null;
            }

            var dayStr = match.Groups[1].Value;
            var monthStr = match.Groups[2].Value.ToLowerInvariant();
            var yearStr = match.Groups[3].Success ? match.Groups[3].Value : null;

            // Parse day
            if (!int.TryParse(dayStr, out int day) || day < 1 || day > 31)
            {
                _logger.LogDebug("Invalid day: {Day}", dayStr);
                return null;
            }

            // Find month using static dictionary
            if (!BulgarianMonths.TryGetValue(monthStr, out int month))
            {
                _logger.LogDebug("Unknown Bulgarian month: {Month}", monthStr);
                return null;
            }

            // Determine year
            int year;
            if (!string.IsNullOrEmpty(yearStr))
            {
                // Year is specified directly
                year = int.Parse(yearStr);
            }
            else
            {
                // No year - determine if it's current or next
                year = today.Year;

                // If the month has passed (less than current), use next year
                if (month < today.Month)
                {
                    year++;
                }
            }

            // Check for validity of the date
            if (!IsValidDate(year, month, day))
            {
                _logger.LogDebug("Invalid date: {Year}-{Month:D2}-{Day:D2}", year, month, day);
                return null;
            }

            var parsedDate = new DateTime(year, month, day);

            _logger.LogDebug("Parsed Bulgarian date '{DateText}' as {ParsedDate}", dateText, parsedDate.ToString("yyyy-MM-dd"));

            return parsedDate;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error parsing single Bulgarian date: {DateText}", dateText);
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

    private bool IsObviouslyNonSofiaEvent(EpaygoEventDto eventDto)
    {
        // Name-based filtering using city names that actually appear in event titles
        if (string.IsNullOrEmpty(eventDto.Name))
        {
            return false; // Can't determine, let it pass to detailed check
        }

        var name = eventDto.Name.ToLowerInvariant();

        // Check for specific city names that commonly appear in event titles
        var nonSofiaCities = new[]
        {
            "велико търново", "veliko tarnovo", "v.turnovo", "в.търново",
            "бургас", "burgas", "летен театър бургас",
            "варна", "varna", 
            "пловдив", "plovdiv",
            "стара загора", "stara zagora",
            "русе", "ruse",
            "плевен", "pleven",
            "каварна", "kavarna",
            "габрово", "gabrovo",
            "видин", "vidin",
            "шумен", "shumen",
            "перник", "pernik",
            "кюстендил", "kyustendil",
            "враца", "vratsa",
            "монтана", "montana",
            "силистра", "silistra"
        };

        foreach (var city in nonSofiaCities)
        {
            if (name.Contains(city))
            {
                _logger.LogDebug("Pre-filtered out non-Sofia event based on city in name: {EventName}", eventDto.Name);
                return true; // This is obviously a non-Sofia event
            }
        }

        // Not obviously non-Sofia, let it pass to detailed check
        return false;
    }
}
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

                                        // Extract date from description
                                        if (!string.IsNullOrEmpty(eventDto.Description))
                                        {
                                            var extractedDate = ExtractDateFromDescription(eventDto.Description);
                                            if (extractedDate.HasValue)
                                            {
                                                eventDto.Date = extractedDate.Value.ToString("yyyy-MM-dd");
                                                _logger.LogDebug("Successfully parsed date: {Date} for event: {EventName}", eventDto.Date, eventDto.Name);
                                            }
                                            else
                                            {
                                                // Fallback: Set to null if no date found
                                                eventDto.Date = null;
                                                _logger.LogWarning("Could not extract date from description for event: {EventName}", eventDto.Name);
                                            }
                                        }
                                        else
                                        {
                                            eventDto.Date = null;
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

    /// <summary>
    /// Extracts date from Bulgarian description text.
    /// Patterns: "на 11 декември 2025 г.", "на 27 юни 2026", "31-ви декември", "20.12", "20 ДЕК 2025", "26 и 27 февруари", итс.
    /// </summary>
    private DateTime? ExtractDateFromDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        try
        {
            var currentDate = DateTime.Today;
            
            // Dictionary of Bulgarian months -> month number
            var bulgarianMonths = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["януари"] = 1, ["ян"] = 1, ["яну"] = 1,
                ["февруари"] = 2, ["фев"] = 2, ["феб"] = 2,
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

            // Pattern 1: "на 11 декември 2025 г." or "на 27 юни 2026"
            var pattern1 = @"на\s+(\d{1,2})\s+([а-яА-Я]+)\s+(\d{4})(?:\s*г\.?)?";
            var match1 = System.Text.RegularExpressions.Regex.Match(description, pattern1, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match1.Success)
            {
                if (TryParseBulgarianDate(match1.Groups[1].Value, match1.Groups[2].Value, match1.Groups[3].Value, bulgarianMonths, out var date1))
                {
                    _logger.LogDebug("Extracted date (pattern 1): {Date} from '{MatchedText}'", date1.ToString("yyyy-MM-dd"), match1.Value);
                    return date1;
                }
            }

            // Pattern 2: "на 23 юли" (without year)
            var pattern2 = @"на\s+(\d{1,2})\s+([а-яА-Я]+)(?!\s+\d{4})";
            var match2 = System.Text.RegularExpressions.Regex.Match(description, pattern2, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match2.Success)
            {
                if (TryParseBulgarianDateWithoutYear(match2.Groups[1].Value, match2.Groups[2].Value, bulgarianMonths, currentDate, out var date2))
                {
                    _logger.LogDebug("Extracted date (pattern 2 - no year): {Date} from '{MatchedText}'", date2.ToString("yyyy-MM-dd"), match2.Value);
                    return date2;
                }
            }

            // Pattern 3: "31-ви декември" (ordinal numbers)
            var pattern3 = @"(\d{1,2})-[а-яА-Я]{1,3}\s+([а-яА-Я]+)(?:\s+(\d{4}))?";
            var match3 = System.Text.RegularExpressions.Regex.Match(description, pattern3, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match3.Success)
            {
                var dayStr = match3.Groups[1].Value;
                var monthStr = match3.Groups[2].Value;
                var yearStr = match3.Groups[3].Value;

                if (string.IsNullOrEmpty(yearStr))
                {
                    if (TryParseBulgarianDateWithoutYear(dayStr, monthStr, bulgarianMonths, currentDate, out var date3))
                    {
                        _logger.LogDebug("Extracted date (pattern 3 - ordinal, no year): {Date} from '{MatchedText}'", date3.ToString("yyyy-MM-dd"), match3.Value);
                        return date3;
                    }
                }
                else
                {
                    if (TryParseBulgarianDate(dayStr, monthStr, yearStr, bulgarianMonths, out var date3))
                    {
                        _logger.LogDebug("Extracted date (pattern 3 - ordinal): {Date} from '{MatchedText}'", date3.ToString("yyyy-MM-dd"), match3.Value);
                        return date3;
                    }
                }
            }

            // Pattern 4: "20.12" (DD.MM format) - FIXED: Ensure day <= 31 and month <= 12
            var pattern4 = @"(\d{1,2})\.(\d{1,2})(?!\d)";
            var matches4 = System.Text.RegularExpressions.Regex.Matches(description, pattern4);

            foreach (System.Text.RegularExpressions.Match match in matches4)
            {
                if (int.TryParse(match.Groups[1].Value, out int firstNum) &&
                    int.TryParse(match.Groups[2].Value, out int secondNum))
                {
                    // Determine which is day and which is month
                    int day, month;
                    
                    // If first number > 12, it must be day, second must be month
                    if (firstNum > 12 && secondNum <= 12)
                    {
                        day = firstNum;
                        month = secondNum;
                    }
                    // If second number > 12, first must be month, second must be day - but this is unusual for DD.MM
                    else if (secondNum > 12 && firstNum <= 12)
                    {
                        // Skip this case as DD.MM format expects day first
                        continue;
                    }
                    // Both numbers <= 12, assume DD.MM format (day first, month second)
                    else if (firstNum <= 31 && secondNum <= 12 && firstNum >= 1 && secondNum >= 1)
                    {
                        day = firstNum;
                        month = secondNum;
                    }
                    else
                    {
                        continue; // Invalid numbers
                    }

                    var year = DetermineYearForDate(currentDate, month);
                    if (IsValidDate(year, month, day))
                    {
                        var date4 = new DateTime(year, month, day);
                        _logger.LogDebug("Extracted date (pattern 4 - DD.MM): {Date} from '{MatchedText}'", date4.ToString("yyyy-MM-dd"), match.Value);
                        return date4;
                    }
                }
            }

            // Pattern 5: "20 ДЕК 2025" (uppercase short month)
            var pattern5 = @"(\d{1,2})\s+([А-Я]{3,})\s+(\d{4})";
            var match5 = System.Text.RegularExpressions.Regex.Match(description, pattern5, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match5.Success)
            {
                if (TryParseBulgarianDate(match5.Groups[1].Value, match5.Groups[2].Value, match5.Groups[3].Value, bulgarianMonths, out var date5))
                {
                    _logger.LogDebug("Extracted date (pattern 5 - short month): {Date} from '{MatchedText}'", date5.ToString("yyyy-MM-dd"), match5.Value);
                    return date5;
                }
            }

            // Pattern 6: Alternative pattern - try without "на"
            // "11 декември 2025 г." or "27 юни 2026"
            var pattern6 = @"(\d{1,2})\s+([а-яА-Я]+)\s+(\d{4})(?:\s*г\.?)?";
            var matches6 = System.Text.RegularExpressions.Regex.Matches(description, pattern6, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in matches6)
            {
                if (TryParseBulgarianDate(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, bulgarianMonths, out var date6))
                {
                    _logger.LogDebug("Extracted date (pattern 6 - without 'на'): {Date} from '{MatchedText}'", date6.ToString("yyyy-MM-dd"), match.Value);
                    return date6;
                }
            }

            // Pattern 7: "26 и 27 февруари" or "26 и 27 февруари 2026" (take first date)
            var pattern7 = @"(\d{1,2})\s+и\s+\d{1,2}\s+([а-яА-Я]+)(?:\s+(\d{4}))?";
            var match7 = System.Text.RegularExpressions.Regex.Match(description, pattern7, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match7.Success)
            {
                var dayStr = match7.Groups[1].Value;
                var monthStr = match7.Groups[2].Value;
                var yearStr = match7.Groups[3].Value;

                if (string.IsNullOrEmpty(yearStr))
                {
                    if (TryParseBulgarianDateWithoutYear(dayStr, monthStr, bulgarianMonths, currentDate, out var date7))
                    {
                        _logger.LogDebug("Extracted date (pattern 7 - DD и DD месец, no year): {Date} from '{MatchedText}'", date7.ToString("yyyy-MM-dd"), match7.Value);
                        return date7;
                    }
                }
                else
                {
                    if (TryParseBulgarianDate(dayStr, monthStr, yearStr, bulgarianMonths, out var date7))
                    {
                        _logger.LogDebug("Extracted date (pattern 7 - DD и DD месец): {Date} from '{MatchedText}'", date7.ToString("yyyy-MM-dd"), match7.Value);
                        return date7;
                    }
                }
            }

            _logger.LogDebug("No valid date found in description: {Description}", description.Substring(0, Math.Min(100, description.Length)));
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting date from description: {Description}", description?.Substring(0, Math.Min(50, description?.Length ?? 0)));
            return null;
        }
    }

    /// <summary>
    /// Helper method to parse Bulgarian date with year
    /// </summary>
    private bool TryParseBulgarianDate(string dayStr, string monthStr, string yearStr, Dictionary<string, int> bulgarianMonths, out DateTime date)
    {
        date = default;

        if (int.TryParse(dayStr, out int day) &&
            int.TryParse(yearStr, out int year) &&
            bulgarianMonths.TryGetValue(monthStr.ToLowerInvariant(), out int month))
        {
            if (IsValidDate(year, month, day))
            {
                date = new DateTime(year, month, day);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Helper method to parse Bulgarian date without year (determines year automatically)
    /// </summary>
    private bool TryParseBulgarianDateWithoutYear(string dayStr, string monthStr, Dictionary<string, int> bulgarianMonths, DateTime currentDate, out DateTime date)
    {
        date = default;

        if (int.TryParse(dayStr, out int day) &&
            bulgarianMonths.TryGetValue(monthStr.ToLowerInvariant(), out int month))
        {
            var year = DetermineYearForDate(currentDate, month);
            
            if (IsValidDate(year, month, day))
            {
                date = new DateTime(year, month, day);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines appropriate year for a given month (current year or next year)
    /// </summary>
    private int DetermineYearForDate(DateTime currentDate, int month)
    {
        // If the month has already passed this year, use next year
        // Otherwise use current year
        if (month < currentDate.Month)
        {
            return currentDate.Year + 1;
        }
        else if (month == currentDate.Month)
        {
            // If it's the same month, we could be more sophisticated here,
            // but for simplicity, assume next year if it's the same month
            return currentDate.Year + 1;
        }
        else
        {
            return currentDate.Year;
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

    private static bool IsValidDate(int year, int month, int day)
    {
        if (year < 2000 || year > 2100 || month < 1 || month > 12 || day < 1 || day > 31)
        {
            return false;
        }

        try
        {
            var _ = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.TicketStation;
using Events.Crawler.Enums;
using Events.Crawler.Services.Helpers;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
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
        return PlaywrightHelper.IsChromiumAvailable();
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

                var chromiumPath = PlaywrightHelper.GetChromiumExecutablePath();
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
                        _logger.LogWarning("TicketStation Timeout waiting for .item elements");
                        await Task.Delay(5000);
                    }

                    await Task.Delay(3000);

                    // Phase 1: Collect cards from all pages (DOM snapshot + tc:gt API)
                    var cardDataList = await CollectAllPagesCardDataAsync(page, url);
                    _logger.LogInformation("Snapshotted {Count} total card references across all pages", cardDataList.Count);

                    // Wait for any pending fetch/network activity from Phase 1 to settle
                    // before starting GotoAsync calls in Phase 2 — prevents ERR_ABORTED
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    await Task.Delay(1000);

                    // Phase 2+3: Process only Sofia events — skip navigation for other cities
                    var events = new List<TicketStationEventDto>();

                    foreach (var cardData in cardDataList)
                    {
                        if (!IsSofiaCity(cardData.City?.Trim().ToLowerInvariant() ?? ""))
                        {
                            _logger.LogDebug("Skipping non-Sofia card: {Name} in {City}", cardData.Name, cardData.City);
                            continue;
                        }

                        try
                        {
                            var extracted = await ExtractEventDetailsAsync(page, cardData);
                            foreach (var eventDto in extracted)
                            {
                                if (!string.IsNullOrEmpty(eventDto.Name))
                                {
                                    events.Add(eventDto);
                                    _logger.LogDebug("Extracted TicketStation event: {Name} on {Date} in {City}", eventDto.Name, eventDto.Date, eventDto.City);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error extracting details for: {Name}", cardData.Name);
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

    private record CardBasicData(string? Name, string? City, string? ImageUrl, string DetailUrl);

    // Collects card data from all pagination pages via Locator clicks.
    // Uses JS eval for pagination detection and href comparison — immune to stale element handles.
    // Deduplicates by DetailUrl since the site may show the same card on multiple pages.
    private async Task<List<CardBasicData>> CollectAllPagesCardDataAsync(IPage page, string baseUrl)
    {
        var allCards = new List<CardBasicData>();

        await DismissCookieConsentAsync(page);

        // Page 1 is already loaded in the browser
        var page1Cards = await SnapshotCardDataAsync(page, baseUrl);
        allCards.AddRange(page1Cards);
        _logger.LogInformation("Page 1: snapshotted {Count} cards", page1Cards.Count);

        var pageNumbers = await GetPaginationPageNumbersAsync(page);
        _logger.LogInformation("Pagination pages found: {Pages}", string.Join(", ", pageNumbers));

        foreach (var pageNum in pageNumbers)
        {
            try
            {
                var hrefsBefore = await GetAllCardHrefsAsync(page);

                // Set up response listener BEFORE clicking — tc:gt may respond before we await it
                var responseTask = page.WaitForResponseAsync(
                    r => r.Url.Contains("tc:gt") && r.Status == 200,
                    new PageWaitForResponseOptions { Timeout = 12000 });

                // Use Mouse.ClickAsync at the element's real coordinates — simulates full mouse interaction
                // (mousedown + mouseup + click). JS link.click() only fires 'click', missing pointer events.
                var paginationLocator = page.Locator("ul.pagination li.page-item a.page-link")
                    .Filter(new LocatorFilterOptions { HasText = pageNum.ToString() })
                    .First;

                // ScrollIntoViewIfNeededAsync ensures the element is in the viewport before clicking.
                // Mouse.ClickAsync uses viewport coordinates — without scrolling, clicks miss off-screen elements.
                await paginationLocator.ScrollIntoViewIfNeededAsync();
                await paginationLocator.ClickAsync();

                // Wait for tc:gt network response — signal that new data has arrived
                try
                {
                    await responseTask;
                    _logger.LogDebug("tc:gt response received for page {Page}", pageNum);
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("No tc:gt response detected after clicking page {Page}", pageNum);
                }

                // Log active pagination page for diagnostics
                var activePage = await page.EvaluateAsync<string?>(@"() => {
                    const active = document.querySelector('ul.pagination li.page-item.active a.page-link');
                    return active ? active.innerText.trim() : null;
                }");
                _logger.LogInformation("Active pagination page after click: {ActivePage} (expected: {Expected})", activePage, pageNum);

                // Wait for DOM cards to update
                await WaitForCardsToRefreshAsync(page, hrefsBefore);

                var pageCards = await SnapshotCardDataAsync(page, baseUrl);
                allCards.AddRange(pageCards);
                _logger.LogInformation("Page {PageNum}: snapshotted {Count} cards", pageNum, pageCards.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error collecting cards from pagination page {Page}", pageNum);
            }
        }

        // Deduplicate by DetailUrl — the site shows the same cards on multiple pages
        var seen = new HashSet<string>();
        var deduplicated = new List<CardBasicData>();
        foreach (var card in allCards)
        {
            if (seen.Add(card.DetailUrl))
                deduplicated.Add(card);
            else
                _logger.LogDebug("Duplicate card skipped: {Name} ({Url})", card.Name, card.DetailUrl);
        }

        if (deduplicated.Count < allCards.Count)
            _logger.LogInformation("Deduplication removed {Count} duplicate cards ({Total} → {Unique})",
                allCards.Count - deduplicated.Count, allCards.Count, deduplicated.Count);

        return deduplicated;
    }

    // NOTE: Accepting cookies may trigger a page reload — waits for .item cards to reappear afterward.
    private async Task DismissCookieConsentAsync(IPage page)
    {
        try
        {
            // Click via JS to avoid Playwright navigation race conditions
            var dismissed = await page.EvaluateAsync<bool>(@"() => {
                const btn = document.querySelector(
                    '#cookiescript_accept, #cookiescript_acceptall, button[data-cs-action=""accept""], .cookiescript_accept'
                );
                if (btn) { btn.click(); return true; }
                const dialog = document.querySelector('div[role=""dialog""][aria-live=""assertive""]');
                if (dialog) { dialog.remove(); return true; }
                return false;
            }");

            if (dismissed)
            {
                // Wait for potential page reload to settle
                await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                try
                {
                    await page.WaitForSelectorAsync(".item", new PageWaitForSelectorOptions { Timeout = 8000 });
                }
                catch (TimeoutException)
                {
                    await Task.Delay(2000);
                }
                _logger.LogInformation("Cookie consent dismissed");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not dismiss cookie consent — will attempt pagination anyway");
        }
    }

    // Returns page numbers from pagination, skipping page 1 (already loaded) and non-numeric entries.
    // Uses JS evaluation to avoid IElementHandle going stale after cookie consent reload.
    private async Task<List<int>> GetPaginationPageNumbersAsync(IPage page)
    {
        var result = new List<int>();

        try
        {
            var texts = await page.EvaluateAsync<string[]>(@"() =>
                Array.from(document.querySelectorAll('ul.pagination li.page-item a.page-link'))
                    .map(a => a.innerText.trim())
            ");

            foreach (var text in texts ?? [])
            {
                if (int.TryParse(text, out var num) && num > 1)
                    result.Add(num);
            }

            result.Sort();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading pagination links");
        }

        return result;
    }

    private async Task<string> GetAllCardHrefsAsync(IPage page)
    {
        try
        {
            // Use specific selector to target only event cards, not other .item elements on the page.
            var hrefs = await page.EvaluateAsync<string[]>(@"() =>
                Array.from(document.querySelectorAll('.item.ev-radius.mb-3'))
                    .map(el => el.getAttribute('href') || '')
                    .filter(h => h !== '')
            ");
            return string.Join("|", hrefs ?? []);
        }
        catch
        {
            return string.Empty;
        }
    }

    // Waits until the full set of card hrefs changes, indicating new page content has loaded.
    // Comparing the full set (not just first card) prevents false negatives when first card is the same across pages.
    private async Task WaitForCardsToRefreshAsync(IPage page, string previousHrefs)
    {
        var timeout = TimeSpan.FromSeconds(10);
        var interval = TimeSpan.FromMilliseconds(500);
        var elapsed = TimeSpan.Zero;

        while (elapsed < timeout)
        {
            await Task.Delay(interval);
            elapsed += interval;

            try
            {
                var currentHrefs = await GetAllCardHrefsAsync(page);
                if (!string.IsNullOrEmpty(currentHrefs) && currentHrefs != previousHrefs)
                    return;
            }
            catch (PlaywrightException)
            {
                // Page is mid-navigation after pagination click — keep waiting
            }
        }

        // Fallback: pagination may not have changed the visible cards
        _logger.LogWarning("Card set did not change after pagination click within {Timeout}s — proceeding anyway", timeout.TotalSeconds);
        await Task.Delay(2000);
    }

    // Loads the main page once and snapshots all card hrefs, names, cities and images in a single pass.
    // This eliminates the need to navigate back to the main page for each card.
    // Uses JS evaluation to extract plain data — immune to stale element handle errors after page reloads.
    private async Task<List<CardBasicData>> SnapshotCardDataAsync(IPage page, string baseUrl)
    {
        var baseUri = new Uri(baseUrl);
        var host = $"{baseUri.Scheme}://{baseUri.Host}";

        // Ensure cards are present before querying
        try
        {
            await page.WaitForSelectorAsync(".item.ev-radius.mb-3", new PageWaitForSelectorOptions { Timeout = 8000 });
        }
        catch (TimeoutException)
        {
            _logger.LogWarning("No .item.ev-radius.mb-3 elements found on page before snapshot");
            return [];
        }

        List<CardRaw>? raw = null;
        try
        {
            var json = await page.EvaluateAsync<string>(@"() =>
                JSON.stringify(Array.from(document.querySelectorAll('.item.ev-radius.mb-3')).map(card => ({
                    href:     card.getAttribute('href') || '',
                    name:     card.querySelector('.item-name') ? card.querySelector('.item-name').innerText.trim() : '',
                    city:     card.querySelector('.item-city') ? card.querySelector('.item-city').innerText.trim() : '',
                    imageUrl: card.querySelector('img') ? (card.querySelector('img').getAttribute('src') || '') : ''
                })))
            ");

            if (!string.IsNullOrWhiteSpace(json))
                raw = System.Text.Json.JsonSerializer.Deserialize<List<CardRaw>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Card snapshot failed");
            return [];
        }

        var result = new List<CardBasicData>();
        foreach (var r in raw ?? [])
        {
            if (r is null || string.IsNullOrEmpty(r.Href)) continue;

            var imageUrl = !string.IsNullOrEmpty(r.ImageUrl) && r.ImageUrl.StartsWith("/") ? $"{host}{r.ImageUrl}" : r.ImageUrl;

            var detailUrl = r.Href.StartsWith("/") ? $"https://ticketstation.bg{r.Href}" : r.Href;
            result.Add(new CardBasicData(r.Name, r.City, imageUrl, detailUrl));
        }

        return result;
    }

    private class CardRaw
    {
        [JsonPropertyName("href")] public string Href { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("city")] public string City { get; set; } = "";
        [JsonPropertyName("imageUrl")] public string ImageUrl { get; set; } = "";
    }

    private class TcGtApiResponse
    {
        [JsonPropertyName("data")] public List<TcGtItem> Data { get; set; } = [];
        [JsonPropertyName("status")] public string Status { get; set; } = "";
    }

    private class TcGtItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("city")] public string City { get; set; } = "";
        [JsonPropertyName("slug")] public string Slug { get; set; } = "";
        [JsonPropertyName("image_thumbnail")] public string ImageThumbnail { get; set; } = "";
        [JsonPropertyName("min_cost")] public string? MinCost { get; set; }
        [JsonPropertyName("max_cost")] public string? MaxCost { get; set; }
        [JsonPropertyName("currency")] public string Currency { get; set; } = "";
    }

    // Navigates to the detail page. If the page contains sub-cards (multiple dates), processes each one.
    // Returns a list: one dto per date slot, or a single dto for single-date events.
    private async Task<List<TicketStationEventDto>> ExtractEventDetailsAsync(IPage page, CardBasicData cardData)
    {
        var results = new List<TicketStationEventDto>();

        // Step 2: Detail page
        await page.GotoAsync(cardData.DetailUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 30000
        });
        await Task.Delay(2000);

        var itemElements = await page.QuerySelectorAllAsync(".item.ev-radius.mb-3");
        if (itemElements.Count == 0)
        {
            _logger.LogWarning("No .item.ev-radius.mb-3 on detail page for: {Name}", cardData.Name);
            results.Add(new TicketStationEventDto { Name = cardData.Name, City = cardData.City, ImageUrl = cardData.ImageUrl, Url = "https://ticketstation.bg" });
            return results;
        }

        // Determine whether these .item elements are sub-cards (date slots) or direct booking links.
        // Sub-cards: .item has no #collapseDesc on this page — clicking them leads to the real booking page.
        // Direct: only one .item with a direct href to the booking page.
        var firstHref = await itemElements[0].GetAttributeAsync("href");
        var hasDescOnCurrentPage = await page.QuerySelectorAsync("#collapseDesc") != null;

        if (!hasDescOnCurrentPage && itemElements.Count > 1)
        {
            // Sub-cards case: this is an event with multiple date slots
            _logger.LogInformation("Event '{Name}' has {Count} date sub-cards — processing each", cardData.Name, itemElements.Count);

            // Snapshot sub-card hrefs and names via JS (immune to stale handles after navigation)
            var subCardJson = await page.EvaluateAsync<string>(@"() =>
                JSON.stringify(Array.from(document.querySelectorAll('.item.ev-radius.mb-3')).map(card => ({
                    href: card.getAttribute('href') || '',
                    name: card.querySelector('.item-name') ? card.querySelector('.item-name').innerText.trim() : ''
                })))
            ");

            var subCards = System.Text.Json.JsonSerializer.Deserialize<List<SubCardRaw>>(subCardJson ?? "[]",
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

            foreach (var sub in subCards)
            {
                if (string.IsNullOrEmpty(sub.Href)) continue;

                var bookingUrl = sub.Href.StartsWith("/") ? $"https://ticketstation.bg{sub.Href}" : sub.Href;
                // Sub-card name is more specific (e.g. includes date label) — prefer it over the parent card name
                var subName = !string.IsNullOrWhiteSpace(sub.Name) ? sub.Name : cardData.Name;

                var dto = await ExtractBookingPageDetailsAsync(page, bookingUrl, new TicketStationEventDto
                {
                    Name = subName,
                    City = cardData.City,
                    ImageUrl = cardData.ImageUrl,
                    Url = "https://ticketstation.bg"
                });

                if (dto != null)
                    results.Add(dto);
            }
        }
        else
        {
            // Single date or direct booking link
            if (string.IsNullOrEmpty(firstHref))
            {
                _logger.LogWarning("No href in .item on detail page for: {Name}", cardData.Name);
                results.Add(new TicketStationEventDto { Name = cardData.Name, City = cardData.City, ImageUrl = cardData.ImageUrl, Url = "https://ticketstation.bg" });
                return results;
            }

            var locationUrl = firstHref.StartsWith("/") ? $"https://ticketstation.bg{firstHref}" : firstHref;
            var dto = await ExtractBookingPageDetailsAsync(page, locationUrl, new TicketStationEventDto
            {
                Name = cardData.Name,
                City = cardData.City,
                ImageUrl = cardData.ImageUrl,
                Url = "https://ticketstation.bg"
            });

            if (dto != null)
                results.Add(dto);
        }

        return results;
    }

    private class SubCardRaw
    {
        [JsonPropertyName("href")] public string Href { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }

    // Navigates to the booking/location page and extracts description, date, venue.
    private async Task<TicketStationEventDto?> ExtractBookingPageDetailsAsync(IPage page, string bookingUrl, TicketStationEventDto eventDto)
    {
        await page.GotoAsync(bookingUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 30000
        });
        await Task.Delay(2000);

        eventDto.TicketUrl = bookingUrl;

        var descEl = await page.QuerySelectorAsync("#collapseDesc");
        if (descEl != null)
            eventDto.Description = (await descEl.InnerTextAsync()).Replace("Виж още", "").Trim();

        var venueLink = await page.QuerySelectorAsync("a.text-wrap");
        if (venueLink != null)
        {
            var venueSpan = await venueLink.QuerySelectorAsync("span");
            if (venueSpan != null)
            {
                var locationText = await venueSpan.InnerTextAsync();
                if (!string.IsNullOrEmpty(eventDto.City))
                    locationText = locationText.Replace(eventDto.City, "").Trim();
                eventDto.Location = locationText;
            }
        }

        if (!string.IsNullOrEmpty(eventDto.Description))
        {
            var extractedDate = ExtractDateFromDescription(eventDto.Description);
            if (extractedDate.HasValue)
            {
                eventDto.Date = extractedDate.Value.ToString("yyyy-MM-dd");
                _logger.LogDebug("Parsed date {Date} for: {Name}", eventDto.Date, eventDto.Name);
            }
            else
            {
                _logger.LogWarning("Could not extract date for: {Name}", eventDto.Name);
            }
        }

        return eventDto;
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
            Name = CleanText(ticketStationEvent.Name) ?? "",
            Description = CleanText(ticketStationEvent.Description),
            StartDate = TryParseEventDate(ticketStationEvent.Date),
            City = CleanText(ticketStationEvent.City) ?? "",
            Location = ticketStationEvent.Location,
            ImageUrl = ticketStationEvent.ImageUrl,
            SourceUrl = ticketStationEvent.Url,
            TicketUrl = ticketStationEvent.TicketUrl,
            //Price = ticketStationEvent.Price,
            //IsFree = ticketStationEvent.Price == null || ticketStationEvent.Price == 0,
            IsFree = false, // by default all events are considered paid. The admin will adjust if needed the free ones
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
    /// Patterns ordered by priority: specific formats first, general formats last
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
                ["януари"] = 1,
                ["ян"] = 1,
                ["яну"] = 1,
                ["февруари"] = 2,
                ["фев"] = 2,
                ["феб"] = 2,
                ["март"] = 3,
                ["мар"] = 3,
                ["април"] = 4,
                ["апр"] = 4,
                ["май"] = 5,
                ["юни"] = 6,
                ["юн"] = 6,
                ["юли"] = 7,
                ["юл"] = 7,
                ["август"] = 8,
                ["авг"] = 8,
                ["септември"] = 9,
                ["сеп"] = 9,
                ["октомври"] = 10,
                ["окт"] = 10,
                ["ноември"] = 11,
                ["ное"] = 11,
                ["декември"] = 12,
                ["дек"] = 12
            };

            // HIGH PRIORITY PATTERNS (specific date formats for events)
            // Pattern 1: "📅 20 ДЕК 2025" (calendar emoji + uppercase short month - very specific for events)
            var pattern1 = @"📅\s*(\d{1,2})\s+([А-Я]{3,})\s+(\d{4})";
            var match1 = System.Text.RegularExpressions.Regex.Match(description, pattern1, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match1.Success)
            {
                if (TryParseBulgarianDate(match1.Groups[1].Value, match1.Groups[2].Value, match1.Groups[3].Value, bulgarianMonths, currentDate, out var date1))
                {
                    return date1;
                }
            }

            // Pattern 2: "20 ДЕК 2025" (uppercase short month - specific for events)
            var pattern2 = @"(\d{1,2})\s+([А-Я]{3,})\s+(\d{4})";
            var match2 = System.Text.RegularExpressions.Regex.Match(description, pattern2, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match2.Success)
            {
                if (TryParseBulgarianDate(match2.Groups[1].Value, match2.Groups[2].Value, match2.Groups[3].Value, bulgarianMonths, currentDate, out var date2))
                {
                    return date2;
                }
            }

            // Pattern 3: "на 11 декември 2025 г." or "на 27 юни 2026"
            var pattern3 = @"на\s+(\d{1,2})\s+([а-яА-Я]+)\s+(\d{4})(?:\s*г\.?)?";
            var match3 = System.Text.RegularExpressions.Regex.Match(description, pattern3, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match3.Success)
            {
                if (TryParseBulgarianDate(match3.Groups[1].Value, match3.Groups[2].Value, match3.Groups[3].Value, bulgarianMonths, currentDate, out var date3))
                {
                    return date3;
                }
            }

            // Pattern 4: "26 и 27 февруари" or "26 и 27 февруари 2026" (take first date)
            var pattern4 = @"(\d{1,2})\s+и\s+\d{1,2}\s+([а-яА-Я]+)(?:\s+(\d{4}))?";
            var match4 = System.Text.RegularExpressions.Regex.Match(description, pattern4, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match4.Success)
            {
                var dayStr = match4.Groups[1].Value;
                var monthStr = match4.Groups[2].Value;
                var yearStr = match4.Groups[3].Value;

                if (string.IsNullOrEmpty(yearStr))
                {
                    if (TryParseBulgarianDateWithoutYear(dayStr, monthStr, bulgarianMonths, currentDate, out var date4))
                    {
                        return date4;
                    }
                }
                else
                {
                    if (TryParseBulgarianDate(dayStr, monthStr, yearStr, bulgarianMonths, currentDate, out var date4))
                    {
                        return date4;
                    }
                }
            }

            // MEDIUM PRIORITY PATTERNS
            // Pattern 5: "на 23 юли" (without year)
            var pattern5 = @"на\s+(\d{1,2})\s+([а-яА-Я]+)(?!\s+\d{4})";
            var match5 = System.Text.RegularExpressions.Regex.Match(description, pattern5, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match5.Success)
            {
                if (TryParseBulgarianDateWithoutYear(match5.Groups[1].Value, match5.Groups[2].Value, bulgarianMonths, currentDate, out var date5))
                {
                    return date5;
                }
            }

            // Pattern 6: "31-ви декември" (ordinal numbers)
            var pattern6 = @"(\d{1,2})-[а-яА-Я]{1,3}\s+([а-яА-Я]+)(?:\s+(\d{4}))?";
            var match6 = System.Text.RegularExpressions.Regex.Match(description, pattern6, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match6.Success)
            {
                var dayStr = match6.Groups[1].Value;
                var monthStr = match6.Groups[2].Value;
                var yearStr = match6.Groups[3].Value;

                if (string.IsNullOrEmpty(yearStr))
                {
                    if (TryParseBulgarianDateWithoutYear(dayStr, monthStr, bulgarianMonths, currentDate, out var date6))
                    {
                        return date6;
                    }
                }
                else
                {
                    if (TryParseBulgarianDate(dayStr, monthStr, yearStr, bulgarianMonths, currentDate, out var date6))
                    {
                        return date6;
                    }
                }
            }

            // Pattern 7: "20.12" (DD.MM format)
            var pattern7 = @"(\d{1,2})\.(\d{1,2})(?!\d)";
            var matches7 = System.Text.RegularExpressions.Regex.Matches(description, pattern7);

            foreach (System.Text.RegularExpressions.Match match in matches7)
            {
                if (int.TryParse(match.Groups[1].Value, out int firstNum) &&
                    int.TryParse(match.Groups[2].Value, out int secondNum))
                {
                    int day, month;

                    if (firstNum > 12 && secondNum <= 12)
                    {
                        day = firstNum;
                        month = secondNum;
                    }
                    else if (firstNum <= 31 && secondNum <= 12 && firstNum >= 1 && secondNum >= 1)
                    {
                        day = firstNum;
                        month = secondNum;
                    }
                    else
                    {
                        continue;
                    }

                    var year = DetermineYearForDate(currentDate, month);
                    if (IsValidDate(year, month, day))
                    {
                        var date7 = new DateTime(year, month, day);
                        return date7;
                    }
                }
            }

            // LOW PRIORITY PATTERNS (general formats - might catch historical dates)
            // Pattern 8: General "DD месец YYYY" - LAST because it catches historical dates
            var pattern8 = @"(\d{1,2})\s+([а-яА-Я]+)\s+(\d{4})(?:\s*г\.?)?";
            var matches8 = System.Text.RegularExpressions.Regex.Matches(description, pattern8, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (System.Text.RegularExpressions.Match match in matches8)
            {
                if (TryParseBulgarianDate(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, bulgarianMonths, currentDate, out var date8))
                {
                    return date8;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting date from description: {Description}", description?.Substring(0, Math.Min(50, description?.Length ?? 0)));
            return null;
        }
    }

    /// <summary>
    /// Helper method to parse Bulgarian date with year and validation against historical dates
    /// </summary>
    private bool TryParseBulgarianDate(string dayStr, string monthStr, string yearStr, Dictionary<string, int> bulgarianMonths, DateTime currentDate, out DateTime date)
    {
        date = default;

        if (int.TryParse(dayStr, out int day) &&
            int.TryParse(yearStr, out int year) &&
            bulgarianMonths.TryGetValue(monthStr.ToLowerInvariant(), out int month))
        {
            if (IsValidDate(year, month, day))
            {
                var parsedDate = new DateTime(year, month, day);

                // Ignore dates more than 3 years in the past (likely historical references)
                if (parsedDate < currentDate.AddYears(-3))
                {
                    _logger.LogDebug("Ignoring historical date (older than 3 years): {Date}", parsedDate.ToString("yyyy-MM-dd"));
                    return false;
                }

                date = parsedDate;
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

        if (int.TryParse(dayStr, out int day) && bulgarianMonths.TryGetValue(monthStr.ToLowerInvariant(), out int month))
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

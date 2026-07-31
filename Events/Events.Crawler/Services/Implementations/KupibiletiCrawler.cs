using Events.Crawler.DTOs.Common;
using Events.Crawler.DTOs.Kupibileti;
using Events.Crawler.Enums;
using Events.Crawler.Services.Helpers;
using Events.Crawler.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace Events.Crawler.Services.Implementations;

public class KupibiletiCrawler : IWebScrapingCrawler
{
    private const string BaseUrl = "https://www.kupibileti.bg";
    private const string EventsListUrl = BaseUrl;
    // FlareSolverr solves the site's Cloudflare challenge (standard Playwright stealth measures were
    // insufficient — see investigation history) and returns a cf_clearance cookie + matching User-Agent,
    // which we then inject into our own Playwright context. Requires a local FlareSolverr container
    // (docker run -d -p 8191:8191 ghcr.io/flaresolverr/flaresolverr:latest); in production this needs
    // to run as a sidecar next to the Azure Function, with the URL below moved to configuration.
    private const string FlareSolverrUrl = "http://localhost:8191/v1";
    private const string CardSelector = ".card";
    private const string CardImageContainerSelector = ".position-relative.mb-3";
    private const string CardDetailLinkSelector = "a.stretched-link[href]";
    private const string SeatsButtonSelector = "#choose_seats_btn";
    private const string EventNameSelector = "[itemprop='name']";
    private const string StartDateSelector = "time[itemprop='startDate']";
    private const string DescriptionCandidateSelector = "div.col-12";
    private const string LocationSelector = "[itemprop='location']";
    private const string ShowLinkContainerSelector = "div.col-xl-3.col-lg-4.col-12";

    private readonly ILogger<KupibiletiCrawler> _logger;
    private static bool _browsersInstalled = false;
    private static readonly object _installLock = new();

    public string SourceName => "kupibileti.bg";
    public CrawlerType CrawlerType => CrawlerType.WebScraping;

    public KupibiletiCrawler(ILogger<KupibiletiCrawler> logger)
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

            var sofiaEvents = await GetKupibiletiEventsAsync();

            result.EventsFound = sofiaEvents.Count;
            result.Events = sofiaEvents;
            result.EventsProcessed = sofiaEvents.Count;
            result.Success = true;

            _logger.LogInformation("[{Source}] Crawled {EventCount} Sofia shows from Kupibileti", SourceName, sofiaEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{Source}] Error crawling Kupibileti.bg", SourceName);
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
            _logger.LogError(ex, "[{Source}] Error extracting elements from {Url} with selector {Selector}", SourceName, url, selector);
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
            _logger.LogError(ex, "[{Source}] Error getting page content from {Url}", SourceName, url);
            throw;
        }
    }

    public bool IsHealthy()
    {
        return PlaywrightHelper.IsChromiumAvailable();
    }

    private async Task<List<CrawledEventDto>> GetKupibiletiEventsAsync()
    {
        // Solve the Cloudflare challenge once via FlareSolverr, then reuse its cf_clearance cookie
        // and User-Agent for our own Playwright session instead of trying to out-stealth Cloudflare
        // ourselves (standard navigator.webdriver masking etc. was insufficient — see PR history).
        var (clearanceCookies, flareSolverrUserAgent) = await SolveCloudflareChallengeAsync(EventsListUrl);

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = new[]
            {
                "--disable-blink-features=AutomationControlled",
                "--disable-dev-shm-usage",
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-web-security",
                "--disable-features=VizDisplayCompositor"
            }
        });
        // A single shared context keeps cookies/cache/connections warm across all pages,
        // while each page is still closed after use to bound its renderer memory lifetime.
        await using var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            Locale = "bg-BG",
            TimezoneId = "Europe/Sofia",
            // Must match the User-Agent FlareSolverr used to obtain cf_clearance — Cloudflare can tie
            // the clearance cookie to the requesting User-Agent, so a mismatch would void it.
            UserAgent = flareSolverrUserAgent
                ?? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept-Language"] = "bg-BG,bg;q=0.9,en;q=0.8",
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
                ["Accept-Encoding"] = "gzip, deflate, br"
            }
        });

        if (clearanceCookies.Count > 0)
            await context.AddCookiesAsync(clearanceCookies);

        // Mask automation fingerprints that bot-detection services (e.g. Cloudflare) check for.
        // Registered on the context (not a single page) so every page it creates — including the
        // detail/show pages opened later via context.NewPageAsync() — inherits the same masking.
        await context.AddInitScriptAsync(@"
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

        var listPage = await context.NewPageAsync();

        var sofiaEvents = new List<CrawledEventDto>();

        try
        {
            await listPage.GotoAsync(EventsListUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 60000 });

            await EnsureRealContentLoadedAsync(listPage, context, EventsListUrl);
            await SimulateHumanBehaviorAsync(listPage);

            var cards = await ExtractCardDataAsync(listPage);
            _logger.LogInformation("[{Source}] Found {CardCount} unique event cards on Kupibileti", SourceName, cards.Count);

            foreach (var card in cards)
            {
                if (string.IsNullOrEmpty(card.DetailUrl))
                    continue;

                var shows = await ExtractShowsForCardAsync(context, card);
                foreach (var show in shows)
                {
                    var crawledEvent = MapToStandardDto(card, show);
                    if (crawledEvent != null)
                        sofiaEvents.Add(crawledEvent);
                }
            }

            _logger.LogInformation("[{Source}] Successfully extracted {EventCount} Sofia shows from Kupibileti", SourceName, sofiaEvents.Count);
            return sofiaEvents;
        }
        finally
        {
            await listPage.CloseAsync();
        }
    }

    // Calls a locally-running FlareSolverr instance to solve the site's Cloudflare challenge and
    // returns the resulting cf_clearance (and related) cookies plus the User-Agent that solved it.
    // Returns an empty cookie list and null User-Agent on any failure, so the caller can fall back
    // to a plain Playwright session (with our own WaitForBotChallengeToClearAsync as a safety net).
    private async Task<(List<Cookie> Cookies, string? UserAgent)> SolveCloudflareChallengeAsync(string url)
    {
        try
        {
            using var httpClient = new HttpClient();
            var requestJson = JsonSerializer.Serialize(new { cmd = "request.get", url, maxTimeout = 60000 });
            using var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync(FlareSolverrUrl, requestContent);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(responseStream);

            var root = document.RootElement;
            if (!string.Equals(root.GetProperty("status").GetString(), "ok", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("[{Source}] FlareSolverr did not report success for {Url}: {Message}",
                    SourceName, url, root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() : "(no message)");
                return (new List<Cookie>(), null);
            }

            var solution = root.GetProperty("solution");
            var userAgent = solution.TryGetProperty("userAgent", out var uaElement) ? uaElement.GetString() : null;

            var cookies = new List<Cookie>();
            if (solution.TryGetProperty("cookies", out var cookiesElement))
            {
                foreach (var cookieElement in cookiesElement.EnumerateArray())
                {
                    cookies.Add(new Cookie
                    {
                        Name = cookieElement.GetProperty("name").GetString() ?? "",
                        Value = cookieElement.GetProperty("value").GetString() ?? "",
                        Domain = cookieElement.TryGetProperty("domain", out var domainEl) ? domainEl.GetString() : null,
                        Path = cookieElement.TryGetProperty("path", out var pathEl) ? pathEl.GetString() : "/",
                        HttpOnly = cookieElement.TryGetProperty("httpOnly", out var httpOnlyEl) && httpOnlyEl.GetBoolean(),
                        Secure = cookieElement.TryGetProperty("secure", out var secureEl) && secureEl.GetBoolean()
                    });
                }
            }

            _logger.LogInformation("[{Source}] FlareSolverr solved Cloudflare challenge for {Url}, got {CookieCount} cookies", SourceName, url, cookies.Count);
            return (cookies, userAgent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[{Source}] FlareSolverr call failed for {Url}, proceeding without clearance cookies", SourceName, url);
            return (new List<Cookie>(), null);
        }
    }

    // Cloudflare (and similar services) serve a "Just a moment..." interstitial while they verify
    // the browser is not a bot, then auto-redirect to the real page once the JS challenge passes.
    // Poll the title for a short window instead of a fixed delay, since the challenge duration varies.
    // Returns whether the challenge cleared within maxAttempts seconds.
    private async Task<bool> WaitForBotChallengeToClearAsync(IPage page, int maxAttempts = 8)
    {
        const int delayMs = 1000;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var title = await page.TitleAsync();
            var lowerTitle = title.ToLowerInvariant();
            // The interstitial's title is served in the request's negotiated locale — "Just a moment..." (en),
            // "Един момент..." (bg), etc. — so match on both the Latin "moment" token and the Cyrillic "момент" one
            // rather than a single hardcoded English string.
            var isChallengePage = lowerTitle.Contains("moment") || lowerTitle.Contains("момент");
            if (!isChallengePage)
                return true;

            await Task.Delay(delayMs);
        }

        return false;
    }

    // Verifies a navigated page actually cleared the Cloudflare challenge. If Cloudflare's adaptive
    // rate-limiting revoked clearance mid-crawl, requests a fresh cf_clearance cookie for this specific
    // URL from FlareSolverr and retries the navigation once, instead of blindly waiting a long fixed
    // timeout (or giving up) on every single page.
    private async Task EnsureRealContentLoadedAsync(IPage page, IBrowserContext context, string url)
    {
        if (await WaitForBotChallengeToClearAsync(page))
            return;

        _logger.LogInformation("[{Source}] Bot challenge stuck for {Url}, refreshing Cloudflare clearance via FlareSolverr", SourceName, url);
        var (cookies, _) = await SolveCloudflareChallengeAsync(url);
        if (cookies.Count == 0)
        {
            _logger.LogWarning("[{Source}] Could not refresh Cloudflare clearance for {Url}, proceeding anyway", SourceName, url);
            return;
        }

        await context.AddCookiesAsync(cookies);
        await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30000 });

        if (!await WaitForBotChallengeToClearAsync(page, maxAttempts: 10))
            _logger.LogWarning("[{Source}] Bot challenge still stuck for {Url} after clearance refresh, proceeding anyway", SourceName, url);
    }

    // Simulate human behavior to help pass behavioral bot-detection checks.
    private async Task SimulateHumanBehaviorAsync(IPage page)
    {
        await page.Mouse.MoveAsync(300, 200, new MouseMoveOptions { Steps = 10 });
        await Task.Delay(1000);
        for (int i = 0; i < 3; i++)
        {
            await page.Mouse.WheelAsync(0, 300);
            await Task.Delay(1000);
        }
    }

    private async Task<List<KupibiletiEventDto>> ExtractCardDataAsync(IPage listPage)
    {
        var cards = new List<KupibiletiEventDto>();
        var seenDetailUrls = new HashSet<string>();

        try
        {
            var cardElements = await listPage.QuerySelectorAllAsync(CardSelector);

            foreach (var cardElement in cardElements)
            {
                try
                {
                    // Skip cards that belong to the top "recommended events" banner (div.text-center) —
                    // those same events are repeated as standalone cards further down the page.
                    var isInBanner = await cardElement.EvaluateAsync<bool>("el => el.closest('.text-center') !== null");
                    if (isInBanner)
                        continue;

                    // a.stretched-link is a sibling of div.position-relative.mb-3 (both direct children
                    // of div.card), not nested inside it — query it directly on the card element.
                    var linkElement = await cardElement.QuerySelectorAsync(CardDetailLinkSelector);
                    if (linkElement == null)
                        continue;

                    var imageContainer = await cardElement.QuerySelectorAsync(CardImageContainerSelector);

                    var href = await linkElement.GetAttributeAsync("href");
                    if (string.IsNullOrEmpty(href))
                        continue;

                    if (!seenDetailUrls.Add(href))
                        continue;

                    var name = await linkElement.InnerTextAsync();

                    var imgElement = imageContainer != null ? await imageContainer.QuerySelectorAsync("img") : null;
                    var imageSrc = imgElement != null ? await imgElement.GetAttributeAsync("src") : null;
                    var imageUrl = string.IsNullOrEmpty(imageSrc)
                        ? null
                        : (imageSrc.StartsWith("http") ? imageSrc : $"{BaseUrl}{imageSrc}");

                    cards.Add(new KupibiletiEventDto
                    {
                        Name = CleanText(name),
                        ImageUrl = imageUrl,
                        DetailUrl = href,
                        SourceUrl = BaseUrl
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "[{Source}] Error extracting card data", SourceName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[{Source}] Error in ExtractCardDataAsync", SourceName);
        }

        return cards;
    }

    private async Task<List<KupibiletiShowDto>> ExtractShowsForCardAsync(IBrowserContext context, KupibiletiEventDto card)
    {
        var shows = new List<KupibiletiShowDto>();
        var detailUrl = card.DetailUrl!.StartsWith("http") ? card.DetailUrl! : $"{BaseUrl}{card.DetailUrl}";

        // Use a fresh page per detail navigation instead of reusing the list page.
        // Reusing one page across dozens of navigations was accumulating renderer
        // memory and crashing the Chromium process in memory-constrained containers.
        var detailPage = await context.NewPageAsync();
        try
        {
            await detailPage.GotoAsync(detailUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30000, Referer = EventsListUrl });
            await EnsureRealContentLoadedAsync(detailPage, context, detailUrl);
            await Task.Delay(300);

            _logger.LogDebug("[{Source}] Loaded detail page for '{EventName}': {Url}", SourceName, card.Name, detailUrl);

            var seatsButton = await detailPage.QuerySelectorAsync(SeatsButtonSelector);
            if (seatsButton != null)
            {
                // Single-showing event: all show data is already on this page.
                var show = await ExtractShowDetailsAsync(detailPage, detailUrl);
                if (show != null)
                    shows.Add(show);

                return shows;
            }

            // Multi-showing hub page: scroll to load every performance, then visit each one's own detail page.
            await ScrollToLoadAllShowsAsync(detailPage);

            var showLinks = await ExtractShowLinksAsync(detailPage);
            _logger.LogDebug("[{Source}] Found {ShowCount} performances for '{EventName}'", SourceName, showLinks.Count, card.Name);

            foreach (var showLink in showLinks)
            {
                var showDetailUrl = showLink.StartsWith("http") ? showLink : $"{BaseUrl}{showLink}";
                var showPage = await context.NewPageAsync();
                try
                {
                    await showPage.GotoAsync(showDetailUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30000, Referer = detailUrl });
                    await EnsureRealContentLoadedAsync(showPage, context, showDetailUrl);
                    await Task.Delay(300);

                    var show = await ExtractShowDetailsAsync(showPage, showDetailUrl);
                    if (show != null)
                        shows.Add(show);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "[{Source}] Error extracting performance details for '{EventName}'", SourceName, card.Name);
                }
                finally
                {
                    await showPage.CloseAsync();
                }
            }

            return shows;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[{Source}] Error extracting shows for event '{EventName}'", SourceName, card.Name);
            return shows;
        }
        finally
        {
            await detailPage.CloseAsync();
        }
    }

    // Extracts show data from the currently loaded page. Assumes the page has already navigated
    // to a single-showing detail page (i.e. one that has SeatsButtonSelector present).
    private async Task<KupibiletiShowDto?> ExtractShowDetailsAsync(IPage page, string ticketUrl)
    {
        try
        {
            var locationElement = await page.QuerySelectorAsync(LocationSelector);
            if (locationElement == null)
            {
                _logger.LogWarning("[{Source}] No location element found on {Url}, skipping show", SourceName, ticketUrl);
                return null;
            }

            var venueNameElement = await locationElement.QuerySelectorAsync(EventNameSelector);
            var venueNameText = venueNameElement != null ? CleanText(await venueNameElement.InnerTextAsync()) : null;

            string? city = null;
            string? location = null;
            if (!string.IsNullOrEmpty(venueNameText))
            {
                var parts = venueNameText.Split(" - ", 2, StringSplitOptions.TrimEntries);
                city = parts[0];
                location = parts.Length > 1 ? parts[1] : null;
            }

            var addressElement = await locationElement.QuerySelectorAsync("[itemprop='streetAddress']");
            var address = addressElement != null ? CleanText(await addressElement.InnerTextAsync()) : null;

            var nameElement = await page.QuerySelectorAsync(EventNameSelector);
            var name = nameElement != null ? CleanText(await nameElement.InnerTextAsync()) : null;

            var startDate = await ExtractStartDateAsync(page);
            var description = await ExtractDescriptionAsync(page);

            return new KupibiletiShowDto
            {
                Name = name,
                Description = description,
                StartDate = startDate,
                TicketUrl = ticketUrl,
                City = city,
                Location = location,
                Address = address
            };
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[{Source}] Error extracting show details from {Url}", SourceName, ticketUrl);
            return null;
        }
    }

    private async Task<DateTime?> ExtractStartDateAsync(IPage page)
    {
        try
        {
            var dateElement = await page.QuerySelectorAsync(StartDateSelector);
            if (dateElement == null)
                return null;

            var datetimeAttr = await dateElement.GetAttributeAsync("datetime");
            if (string.IsNullOrWhiteSpace(datetimeAttr))
                return null;

            return DateTimeOffset.Parse(datetimeAttr, CultureInfo.InvariantCulture, DateTimeStyles.None).DateTime;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[{Source}] Error extracting start date", SourceName);
            return null;
        }
    }

    private async Task<string?> ExtractDescriptionAsync(IPage page)
    {
        try
        {
            var candidates = await page.QuerySelectorAllAsync(DescriptionCandidateSelector);
            string? longestText = null;

            foreach (var candidate in candidates)
            {
                var text = await candidate.InnerTextAsync();
                if (string.IsNullOrWhiteSpace(text))
                    continue;

                if (longestText == null || text.Length > longestText.Length)
                    longestText = text;
            }

            return CleanText(longestText);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[{Source}] Error extracting description", SourceName);
            return null;
        }
    }

    private async Task<List<string>> ExtractShowLinksAsync(IPage page)
    {
        var links = new List<string>();

        try
        {
            var showContainers = await page.QuerySelectorAllAsync(ShowLinkContainerSelector);

            foreach (var container in showContainers)
            {
                var linkElement = await container.QuerySelectorAsync("a[href]");
                if (linkElement == null)
                    continue;

                var href = await linkElement.GetAttributeAsync("href");
                if (!string.IsNullOrEmpty(href))
                    links.Add(href);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[{Source}] Error extracting performance links", SourceName);
        }

        return links;
    }

    private async Task ScrollToLoadAllShowsAsync(IPage page)
    {
        var previousHeight = 0;
        var maxScrollAttempts = 50;
        var scrollAttempts = 0;

        while (scrollAttempts < maxScrollAttempts)
        {
            var currentHeight = await page.EvaluateAsync<int>("document.body.scrollHeight");

            if (currentHeight == previousHeight)
            {
                _logger.LogDebug("[{Source}] Reached end of performances list after {Attempts} scroll attempts", SourceName, scrollAttempts);
                break;
            }

            previousHeight = currentHeight;
            await page.EvaluateAsync("window.scrollBy(0, window.innerHeight)");
            await Task.Delay(500);
            scrollAttempts++;
        }
    }

    private CrawledEventDto? MapToStandardDto(KupibiletiEventDto card, KupibiletiShowDto show)
    {
        var isSofia = !string.IsNullOrEmpty(show.City)
            && (show.City.Contains("София", StringComparison.OrdinalIgnoreCase) || show.City.Contains("Sofia", StringComparison.OrdinalIgnoreCase));

        if (!isSofia)
        {
            _logger.LogDebug("[{Source}] Skipping non-Sofia show for '{EventName}': City='{City}'", SourceName, card.Name, show.City);
            return null;
        }

        return new CrawledEventDto
        {
            ExternalId = GenerateEventId(show.Name ?? card.Name, show.TicketUrl),
            Source = SourceName,
            Name = show.Name ?? card.Name ?? "",
            Description = show.Description,
            StartDate = show.StartDate,
            City = "София",
            Location = show.Location ?? "",
            ImageUrl = card.ImageUrl,
            SourceUrl = card.SourceUrl,
            TicketUrl = show.TicketUrl,
            IsFree = false,
            RawData = new Dictionary<string, object>
            {
                ["address"] = show.Address ?? "",
                ["location"] = show.Location ?? "",
                ["extraction_method"] = "Playwright_Kupibileti_ShowDetails",
                ["crawled_from"] = EventsListUrl
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
                _logger.LogInformation("[{Source}] Checking Playwright browser installation...", SourceName);

                var chromiumPath = GetChromiumPath();
                if (string.IsNullOrEmpty(chromiumPath) || !File.Exists(chromiumPath))
                {
                    _logger.LogWarning("[{Source}] Playwright browsers not found. Attempting to install...", SourceName);
                    InstallPlaywrightBrowsers();
                }

                _browsersInstalled = true;
                _logger.LogInformation("[{Source}] Playwright browsers are ready", SourceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Source}] Failed to ensure Playwright browsers are installed", SourceName);
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
            _logger.LogError(ex, "[{Source}] Error installing Playwright browsers", SourceName);
            throw;
        }
    }

    private string? GetChromiumPath() => PlaywrightHelper.GetChromiumExecutablePath();
}

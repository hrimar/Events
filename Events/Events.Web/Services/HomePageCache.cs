using System.Globalization;
using Events.Services.Caching;
using Events.Services.Time;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Events.Web.Services;

public interface IHomePageCache
{
    // Returns the cached data of a homepage section for today's Sofia date, loading it once when missing.
    // The data is shared across requests and must not be modified by callers. Data with localized
    // texts must include the UI culture in the section name (see HomePageCache.ForCulture).
    Task<T> GetOrCreateAsync<T>(string section, TimeSpan duration, Func<Task<T>> factory);
}

// Short-lived cache for homepage section data (GS-REQ-007). The key includes the Sofia date, so the
// sections switch to the new day at midnight. Entries are evicted early through IEventCacheInvalidator,
// which event, tag and quick filter changes already trigger (criteria 2 and 5).
public sealed class HomePageCache : IHomePageCache
{
    // Event-based sections (featured, tonight, weekend, free, counts).
    public static readonly TimeSpan EventsDuration = TimeSpan.FromMinutes(5);

    // Aggregates that change slowly (top categories, top venues) - at most once per hour (7.8.2).
    public static readonly TimeSpan AggregatesDuration = TimeSpan.FromHours(1);

    private const string KeyPrefix = "HomePage";

    private readonly IMemoryCache _cache;
    private readonly IEventCacheInvalidator _cacheInvalidator;
    private readonly ISofiaClock _clock;

    public HomePageCache(IMemoryCache cache, IEventCacheInvalidator cacheInvalidator, ISofiaClock clock)
    {
        _cache = cache;
        _cacheInvalidator = cacheInvalidator;
        _clock = clock;
    }

    // "WeeklyEvents" -> "WeeklyEvents|bg-BG", for sections whose data contains localized texts.
    public static string ForCulture(string section) => $"{section}|{CultureInfo.CurrentUICulture.Name}";

    public async Task<T> GetOrCreateAsync<T>(string section, TimeSpan duration, Func<Task<T>> factory)
    {
        var cacheKey = $"{KeyPrefix}|{section}|{_clock.Today:yyyy-MM-dd}";

        // Lazy<Task<T>> so concurrent requests await one in-flight load (same pattern as EventService).
        var lazyResult = _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.SetAbsoluteExpiration(duration);
            entry.AddExpirationToken(new CancellationChangeToken(_cacheInvalidator.Token));
            return new Lazy<Task<T>>(factory);
        })!;

        try
        {
            return await lazyResult.Value;
        }
        catch
        {
            // A failed load must not stay cached for the whole duration.
            _cache.Remove(cacheKey);
            throw;
        }
    }
}

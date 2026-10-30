using Events.Services.Caching;
using Events.Services.Time;
using Events.Web.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Events.Web.Tests.Services;

public class HomePageCacheTests
{
    private sealed class FixedSofiaClock : ISofiaClock
    {
        public DateTime Now { get; set; } = new(2026, 9, 25, 18, 0, 0);
        public DateTime Today => Now.Date;
    }

    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly EventCacheInvalidator _invalidator = new();
    private readonly FixedSofiaClock _clock = new();

    private HomePageCache CreateCache() => new(_memoryCache, _invalidator, _clock);

    [Fact]
    public async Task GetOrCreateAsync_SameSectionAndDay_LoadsOnce()
    {
        var cache = CreateCache();
        var loads = 0;

        await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));
        var result = await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));

        Assert.Equal(1, result);
        Assert.Equal(1, loads);
    }

    [Fact]
    public async Task GetOrCreateAsync_AfterInvalidate_LoadsAgain()
    {
        var cache = CreateCache();
        var loads = 0;

        await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));
        _invalidator.Invalidate();
        var result = await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetOrCreateAsync_NextSofiaDay_LoadsAgain()
    {
        var cache = CreateCache();
        var loads = 0;

        await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));
        _clock.Now = _clock.Now.AddDays(1);
        var result = await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(++loads));

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetOrCreateAsync_FailedLoad_IsNotCached()
    {
        var cache = CreateCache();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            cache.GetOrCreateAsync<int>("Section", HomePageCache.EventsDuration, () => throw new InvalidOperationException()));
        var result = await cache.GetOrCreateAsync("Section", HomePageCache.EventsDuration, () => Task.FromResult(7));

        Assert.Equal(7, result);
    }
}

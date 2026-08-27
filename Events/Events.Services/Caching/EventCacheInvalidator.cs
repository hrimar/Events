namespace Events.Services.Caching;

/// <summary>
/// Coordinates cache invalidation for cached Event query results. A single shared
/// CancellationToken is handed out to every cache entry as an expiration dependency;
/// calling Invalidate() cancels it, which immediately evicts all entries depending on
/// it, then swaps in a fresh token for subsequent cache writes.
/// </summary>
public interface IEventCacheInvalidator
{
    CancellationToken Token { get; }
    void Invalidate();
}

public class EventCacheInvalidator : IEventCacheInvalidator
{
    private CancellationTokenSource _cts = new();

    public CancellationToken Token => _cts.Token;

    public void Invalidate()
    {
        var previous = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
        previous.Cancel();
        previous.Dispose();
    }
}

using Events.Services.Import.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Events.Web.Areas.Admin.Services;

/// <summary>
/// Holds a parsed import batch in memory between the Upload/Preview/Confirm steps while the admin
/// reviews it, keyed per-user so two admins' in-progress imports can never collide or leak.
/// </summary>
public class EventImportBatchCache
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    private readonly IMemoryCache _cache;

    public EventImportBatchCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void Store(string userKey, EventImportBatch batch) => _cache.Set(Key(userKey, batch.BatchId), batch, Ttl);

    public EventImportBatch? Get(string userKey, Guid batchId) =>
        _cache.TryGetValue(Key(userKey, batchId), out EventImportBatch? batch) ? batch : null;

    public void Remove(string userKey, Guid batchId) => _cache.Remove(Key(userKey, batchId));

    private static string Key(string userKey, Guid batchId) => $"eventimport:{userKey}:{batchId}";
}

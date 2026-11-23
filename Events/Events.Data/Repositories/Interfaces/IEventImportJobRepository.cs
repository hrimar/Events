using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

// Excel/CSV import image jobs and their report lines (GS-REQ-009, 9.3.3, 9.3.4).
public interface IEventImportJobRepository
{
    // Saves a new job together with its items.
    Task AddAsync(EventImportJob job, CancellationToken cancellationToken = default);

    // Job without items (progress polling). Read-only.
    Task<EventImportJob?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Job with all items ordered by row number (report page and CSV). Read-only.
    Task<EventImportJob?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default);

    // Newest jobs first, without items. Read-only.
    Task<List<EventImportJob>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    // Ids of jobs that are Queued or Running, oldest first (resumed by the worker on start).
    Task<List<int>> GetUnfinishedJobIdsAsync(CancellationToken cancellationToken = default);

    // Marks the job as Running (StartedAtUtc is kept from the first start). A Completed job may run again for
    // "Retry remaining" on the report. Returns false when the job does not exist.
    Task<bool> StartAsync(int jobId, DateTime utcNow, CancellationToken cancellationToken = default);

    // Ids of the job's items that still wait for the worker, by row number.
    Task<List<int>> GetPendingItemIdsAsync(int jobId, CancellationToken cancellationToken = default);

    // Tracked item, so the worker can change it and call SaveChangesAsync.
    Task<EventImportJobItem?> GetItemForUpdateAsync(int itemId, CancellationToken cancellationToken = default);

    Task CompleteAsync(int jobId, DateTime utcNow, CancellationToken cancellationToken = default);

    // Persists changes made to items loaded through this repository.
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

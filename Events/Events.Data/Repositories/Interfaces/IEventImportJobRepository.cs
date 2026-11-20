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
}

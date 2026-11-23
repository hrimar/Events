using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class EventImportJobRepository : IEventImportJobRepository
{
    private readonly EventsDbContext _context;

    public EventImportJobRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EventImportJob job, CancellationToken cancellationToken = default)
    {
        _context.EventImportJobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventImportJob?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<EventImportJob?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobs
            .AsNoTracking()
            .Include(j => j.Items.OrderBy(i => i.RowNumber))
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<List<EventImportJob>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobs
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<int>> GetUnfinishedJobIdsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobs
            .AsNoTracking()
            .Where(j => j.Status != EventImportJobStatus.Completed)
            .OrderBy(j => j.CreatedAtUtc)
            .Select(j => j.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> StartAsync(int jobId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var job = await _context.EventImportJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job == null)
        {
            return false;
        }

        job.Status = EventImportJobStatus.Running;
        job.StartedAtUtc ??= utcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<int>> GetPendingItemIdsAsync(int jobId, CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobItems
            .AsNoTracking()
            .Where(i => i.JobId == jobId && i.Outcome == EventImportImageOutcome.Pending)
            .OrderBy(i => i.RowNumber)
            .Select(i => i.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<EventImportJobItem?> GetItemForUpdateAsync(int itemId, CancellationToken cancellationToken = default)
    {
        return await _context.EventImportJobItems.FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
    }

    public async Task CompleteAsync(int jobId, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var job = await _context.EventImportJobs.FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);
        if (job == null)
        {
            return;
        }

        job.Status = EventImportJobStatus.Completed;
        job.CompletedAtUtc = utcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

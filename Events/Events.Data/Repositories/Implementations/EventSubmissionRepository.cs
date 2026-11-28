using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class EventSubmissionRepository : IEventSubmissionRepository
{
    private readonly EventsDbContext _context;

    public EventSubmissionRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EventSubmission submission, CancellationToken cancellationToken = default)
    {
        _context.EventSubmissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<EventSubmission?> GetDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EventSubmissions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Dates.OrderBy(d => d.Date))
            .Include(s => s.Actions.OrderBy(a => a.PerformedAtUtc))
            .Include(s => s.Category)
            .Include(s => s.SubCategory)
            .Include(s => s.CanonicalVenue)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<EventSubmission?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EventSubmissions
            .Include(s => s.Dates)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<EventSubmission>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.EventSubmissions
            .AsNoTracking()
            .Include(s => s.Dates.OrderBy(d => d.Date))
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAtUtc)
            .ThenByDescending(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EventSubmission>> GetByStatusAsync(
        EventSubmissionStatus status, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.EventSubmissions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.Dates.OrderBy(d => d.Date))
            .Include(s => s.CanonicalVenue)
            .Include(s => s.Category)
            .Where(s => s.Status == status)
            .OrderBy(s => s.SubmittedAtUtc)
            .ThenBy(s => s.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<EventSubmissionStatus, int>> CountByStatusAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EventSubmissions
            .AsNoTracking()
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

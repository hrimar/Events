using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class EventTagRepository : IEventTagRepository
{
    private readonly EventsDbContext _context;

    public EventTagRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task BulkAddEventTagsAsync(List<EventTag> eventTags)
    {
        if (!eventTags.Any()) return;

        // Simplified deduplication logic to avoid EF Core LINQ translation issues
        var eventIds = eventTags.Select(et => et.EventId).Distinct().ToList();
        var tagIds = eventTags.Select(et => et.TagId).Distinct().ToList();

        // Get existing EventTag pairs for these events and tags
        var existingEventTags = await _context.EventTags
            .Where(et => eventIds.Contains(et.EventId) && tagIds.Contains(et.TagId))
            .Select(et => new { et.EventId, et.TagId })
            .ToListAsync();

        var existingSet = existingEventTags.ToHashSet();

        // Filter out duplicates
        var newEventTags = eventTags
            .Where(et => !existingSet.Contains(new { et.EventId, et.TagId }))
            .ToList();

        if (newEventTags.Any())
        {
            _context.EventTags.AddRange(newEventTags);
            await _context.SaveChangesAsync();
        }
    }

    public async Task BulkRemoveEventTagsByEventIdAsync(int eventId)
    {
        var eventTags = await _context.EventTags
            .Where(et => et.EventId == eventId)
            .ToListAsync();

        if (eventTags.Any())
        {
            _context.EventTags.RemoveRange(eventTags);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<int, List<string>>> GetEventTagsBulkAsync(List<int> eventIds)
    {
        if (!eventIds.Any()) return new Dictionary<int, List<string>>();

        var eventTags = await _context.EventTags
            .Where(et => eventIds.Contains(et.EventId))
            .Include(et => et.Tag)
            .ToListAsync();

        return eventTags
            .GroupBy(et => et.EventId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(et => et.Tag.Name).ToList()
            );
    }

    public async Task<List<EventTag>> GetEventTagsByEventIdAsync(int eventId)
    {
        return await _context.EventTags
            .Where(et => et.EventId == eventId)
            .Include(et => et.Tag)
            .ToListAsync();
    }

    public async Task<bool> EventTagExistsAsync(int eventId, int tagId)
    {
        return await _context.EventTags
            .AnyAsync(et => et.EventId == eventId && et.TagId == tagId);
    }

    public async Task RemoveEventTagsByTagIdsAsync(IEnumerable<int> tagIds)
    {
        var tagIdList = tagIds?.Distinct().ToList() ?? new List<int>();
        if (!tagIdList.Any()) return;

        var eventTags = await _context.EventTags
            .Where(et => tagIdList.Contains(et.TagId))
            .ToListAsync();

        if (eventTags.Any())
        {
            _context.EventTags.RemoveRange(eventTags);
            await _context.SaveChangesAsync();
        }
    }
}
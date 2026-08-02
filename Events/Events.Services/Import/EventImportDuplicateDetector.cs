using Events.Services.Import.Models;
using Events.Services.Interfaces;

namespace Events.Services.Import;

public class EventImportDuplicateDetector : IEventImportDuplicateDetector
{
    private readonly IEventService _eventService;

    public EventImportDuplicateDetector(IEventService eventService)
    {
        _eventService = eventService;
    }

    public async Task<(bool IsDuplicate, int? ExistingEventId)> FindExistingDuplicateAsync(ImportRowResult row, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(row.Name))
        {
            return (false, null);
        }

        // TicketUrl uniquely identifies a specific showing when present, so check it first —
        // before the date-based fast path — mirroring the crawler's duplicate-detection cascade.
        if (!string.IsNullOrEmpty(row.TicketUrl))
        {
            var candidatesByName = await _eventService.FindEventsByNameAsync(row.Name);
            var byTicketUrl = candidatesByName.FirstOrDefault(e => e.TicketUrl == row.TicketUrl);
            if (byTicketUrl != null)
            {
                return (true, byTicketUrl.Id);
            }
        }

        if (row.Date.HasValue)
        {
            // A row with a known date is only a duplicate if an existing event shares both Name
            // and Date. Falling back to a name-only match here would misflag every later
            // occurrence of a recurring event (e.g. a monthly club night) as a duplicate of the
            // first one, since a same-named event on a different date is a legitimate distinct
            // occurrence, not a re-import of the same row.
            var eventDate = row.Date.Value.Date;
            var byDate = await _eventService.GetEventsByDateRangeAsync(eventDate, eventDate.AddDays(1));
            var match = byDate.FirstOrDefault(e => e.Name.Equals(row.Name, StringComparison.OrdinalIgnoreCase));
            return match != null ? (true, match.Id) : (false, null);
        }

        // No date at all — fall back to a name-only match (mirrors the crawler's behavior for
        // genuinely dateless rows, where Name is the only signal available).
        var byName = await _eventService.FindEventByNameAsync(row.Name);
        return byName != null ? (true, byName.Id) : (false, null);
    }

    public void DetectIntraBatchDuplicates(IReadOnlyList<ImportRowResult> rows)
    {
        var seen = new Dictionary<(string Name, DateTime? Date), ImportRowResult>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Name))
            {
                continue;
            }

            var key = (row.Name.Trim().ToLowerInvariant(), row.Date);

            if (seen.TryGetValue(key, out var firstRow))
            {
                row.IsDuplicate = true;
                row.DuplicateOfRowNumber = firstRow.RowNumber;
                row.AddWarning($"Duplicate of row {firstRow.RowNumber} in this file (same Name + Date).");
            }
            else
            {
                seen[key] = row;
            }
        }
    }
}

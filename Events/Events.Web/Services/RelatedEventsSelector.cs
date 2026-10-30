using Events.Models.Entities;

namespace Events.Web.Services;

// "You might like" on the event page (GS-REQ-008, 8.1): up to 4 events without the current one, without another
// performance of it on the same day (same title and date) and without such repeats among themselves.
// Pure function over the candidates, so the rule is testable without a database.
public static class RelatedEventsSelector
{
    public const int Count = 4;

    // Candidates read for the selection: the current event and its other performances may be among them.
    public const int CandidateCount = Count * 3;

    public static IReadOnlyList<Event> Select(IEnumerable<Event> candidates, Event current, int count = Count)
    {
        var seen = new HashSet<(string Name, DateTime Day)> { KeyOf(current) };

        return candidates
            .Where(candidate => candidate.Id != current.Id && seen.Add(KeyOf(candidate)))
            .Take(count)
            .ToList();
    }

    private static (string Name, DateTime Day) KeyOf(Event eventEntity) =>
        ((eventEntity.Name ?? string.Empty).Trim().ToUpperInvariant(), eventEntity.Date.Date);
}

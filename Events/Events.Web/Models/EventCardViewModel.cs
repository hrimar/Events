using System.Globalization;

namespace Events.Web.Models;

// Data for the event card components (GS-REQ-006, 6.5): standard, compact and horizontal card.
// One card = one performance; performances of the same event are not merged.
public class EventCardViewModel
{
    private const string DefaultImageUrl = "/images/default_event_image.jpeg";

    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = DefaultImageUrl;
    public string? ThumbnailUrl { get; init; }
    public string Venue { get; init; } = string.Empty;
    public bool IsFree { get; init; }
    public string? CategoryName { get; init; }
    public string? SubCategoryName { get; init; }
    public DateTime Date { get; init; }

    // Start time for the time badge ("19:00"); null when the time is unknown, so no badge is shown
    public string? TimeText { get; init; }

    public bool HasKnownTime => TimeText is not null;

    // Past event (GS-BUG-002): the card shows the "Past" label
    public bool IsPast { get; init; }

    // The badge includes the date in lists that are not split by day (search, venue page, paged list);
    // under a day heading only the time is shown
    public bool ShowDate { get; init; } = true;

    public static EventCardViewModel FromEvent(EventViewModel eventModel, bool showDate = true) => new()
    {
        Id = eventModel.Id,
        Name = eventModel.Name,
        ImageUrl = string.IsNullOrWhiteSpace(eventModel.ImageUrl) ? DefaultImageUrl : eventModel.ImageUrl,
        ThumbnailUrl = eventModel.ThumbnailUrl,
        Venue = eventModel.Location,
        IsFree = eventModel.IsFree,
        CategoryName = eventModel.CategoryName,
        SubCategoryName = eventModel.SubCategoryName,
        Date = eventModel.Date.Date,
        TimeText = eventModel.HasKnownTime
            ? (eventModel.StartTime ?? eventModel.Date.TimeOfDay).ToString(@"hh\:mm", CultureInfo.InvariantCulture)
            : null,
        IsPast = eventModel.IsPastEvent,
        ShowDate = showDate
    };

    public static List<EventCardViewModel> FromEvents(IEnumerable<EventViewModel> events, bool showDate = true) =>
        events.Select(e => FromEvent(e, showDate)).ToList();
}

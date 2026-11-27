using Events.Models.Entities;
using Events.Models.Enums;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Localization;

namespace Events.Web.Models;

public class EventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // The Bulgarian title in both languages, so performances of one event are grouped the same way on BG and EN
    // pages even when only some of them have an English title (GS-REQ-010, 10.4).
    public string BulgarianName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public string FormattedDate => Date.ToString("dd.MM.yyyy");
    public bool HasKnownTime => EventDateTimeHelper.HasKnownTime(Date, StartTime);
    public bool IsPastEvent { get; set; }
    public string FormattedTime => StartTime?.ToString(@"hh\:mm") ?? Date.ToString("HH:mm");
    public string City { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TicketUrl { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    //public string PriceDisplay => IsFree ? "Free" : Price?.ToString("F0") + " EUR";
    public string PriceDisplay => IsFree ? "Free" : "";
    public string? CategoryName { get; set; }
    public string? SubCategoryName { get; set; }
    public EventStatus Status { get; set; }
    public int? CanonicalVenueId { get; set; }
    public string? VenueSlug { get; set; }
    public List<string> Tags { get; set; } = new();
    
    public string ShortDescription => Description?.Length > 150
        ? Description[..147] + "..."
        : Description ?? "";

    public string DefaultImage => ImageUrl ?? "/images/default_event_image.jpeg";

    public bool HasTicketUrl => !string.IsNullOrEmpty(TicketUrl);

    // Factory method for mapping from Entity.
    // sofiaToday comes from ISofiaClock.Today and drives the "past event" label.
    public static EventViewModel FromEntity(Event eventEntity, DateTime sofiaToday) =>
        Populate(new EventViewModel(), eventEntity, sofiaToday);

    // Shared entity-to-view-model mapping, so subclasses (e.g. EventDetailsViewModel)
    // can add their own fields without duplicating this mapping.
    // Texts are in the language of the current request (GS-REQ-010, 10.4); cached lists of these models must use
    // a per-culture cache key (HomePageCache.ForCulture).
    protected static T Populate<T>(T model, Event eventEntity, DateTime sofiaToday) where T : EventViewModel
    {
        var isEnglish = CultureHelper.IsEnglish();

        model.Id = eventEntity.Id;
        model.Name = eventEntity.LocalizedName(isEnglish);
        model.BulgarianName = eventEntity.Name;
        model.Date = eventEntity.Date;
        model.StartTime = eventEntity.StartTime;
        model.City = eventEntity.LocalizedCity(isEnglish);
        model.Location = eventEntity.LocalizedLocation(isEnglish);
        model.Description = eventEntity.LocalizedDescription(isEnglish);
        model.ImageUrl = eventEntity.ImageUrl;
        model.ThumbnailUrl = eventEntity.ThumbnailUrl;
        model.TicketUrl = eventEntity.TicketUrl;
        model.IsFree = eventEntity.IsFree;
        //model.Price = eventEntity.Price;
        model.CategoryName = eventEntity.Category?.Name;
        model.SubCategoryName = eventEntity.SubCategory?.Name;
        model.Status = eventEntity.Status;
        model.CanonicalVenueId = eventEntity.CanonicalVenueId;
        model.VenueSlug = eventEntity.CanonicalVenue?.Slug;
        model.Tags = eventEntity.EventTags?.Select(et => et.Tag.Name).ToList() ?? new List<string>();
        model.IsPastEvent = EventDateTimeHelper.IsPastEvent(eventEntity.Date, sofiaToday);
        return model;
    }

    // Extension method for bulk mapping
    public static List<EventViewModel> FromEntities(IEnumerable<Event> events, DateTime sofiaToday)
    {
        return events.Select(e => FromEntity(e, sofiaToday)).ToList();
    }
}
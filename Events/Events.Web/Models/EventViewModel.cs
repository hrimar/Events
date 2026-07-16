using Events.Models.Entities;
using Events.Models.Enums;
using Events.Web.Infrastructure.JsonLd;

namespace Events.Web.Models;

public class EventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public string FormattedDate => Date.ToString("dd.MM.yyyy");
    public bool HasKnownTime => EventDateTimeHelper.HasKnownTime(Date, StartTime);
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

    // Factory method for mapping from Entity
    public static EventViewModel FromEntity(Event eventEntity) => Populate(new EventViewModel(), eventEntity);

    // Shared entity-to-view-model mapping, so subclasses (e.g. EventDetailsViewModel)
    // can add their own fields without duplicating this mapping.
    protected static T Populate<T>(T model, Event eventEntity) where T : EventViewModel
    {
        model.Id = eventEntity.Id;
        model.Name = eventEntity.Name;
        model.Date = eventEntity.Date;
        model.StartTime = eventEntity.StartTime;
        model.City = eventEntity.City;
        model.Location = eventEntity.Location;
        model.Description = eventEntity.Description;
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
        return model;
    }

    // Extension method for bulk mapping
    public static List<EventViewModel> FromEntities(IEnumerable<Event> events)
    {
        return events.Select(FromEntity).ToList();
    }
}
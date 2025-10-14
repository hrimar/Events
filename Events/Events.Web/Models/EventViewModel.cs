using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Web.Models;

public class EventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string FormattedDate => Date.ToString("dd.MM.yyyy");
    public string FormattedTime => Date.ToString("HH:mm");
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    public string PriceDisplay => IsFree ? "Free" : Price?.ToString("F0") + " BGN";
    public string? CategoryName { get; set; }
    public string? SubCategoryName { get; set; }
    public EventStatus Status { get; set; }
    public List<string> Tags { get; set; } = new();
    
    public string ShortDescription => Description?.Length > 150
        ? Description[..147] + "..."
        : Description ?? "";

    public string DefaultImage => ImageUrl ?? "/images/default-event.jpg";

    public bool HasTicketUrl => !string.IsNullOrEmpty(TicketUrl);

    // Factory method for mapping from Entity
    public static EventViewModel FromEntity(Event eventEntity)
    {
        return new EventViewModel
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Date = eventEntity.Date,
            Location = eventEntity.Location,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,
            TicketUrl = eventEntity.TicketUrl,
            IsFree = eventEntity.IsFree,
            Price = eventEntity.Price,
            CategoryName = eventEntity.Category?.Name,
            SubCategoryName = eventEntity.SubCategory?.Name,
            Status = eventEntity.Status,
            Tags = eventEntity.EventTags?.Select(et => et.Tag.Name).ToList() ?? new List<string>()
        };
    }

    // Extension method for bulk mapping
    public static List<EventViewModel> FromEntities(IEnumerable<Event> events)
    {
        return events.Select(FromEntity).ToList();
    }
}
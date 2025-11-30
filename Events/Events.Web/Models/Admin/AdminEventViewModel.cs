using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Web.Models.Admin;

public class AdminEventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string FormattedDate => Date.ToString("dd.MM.yyyy");
    public string FormattedTime => Date.ToString("HH:mm");
    public string City { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    public bool IsFeatured { get; set; }
    public string PriceDisplay => IsFree ? "Free" : Price?.ToString("F0") + " EUR";
    public string? CategoryName { get; set; }
    public int CategoryId { get; set; }
    public string? SubCategoryName { get; set; }
    public int? SubCategoryId { get; set; }
    public EventStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? SourceUrl { get; set; }

    public string ShortDescription => Description?.Length > 100 
        ? Description[..97] + "..." 
        : Description ?? "";

    public string DefaultImage => ImageUrl ?? "/images/default-event.jpg";
    public bool HasTicketUrl => !string.IsNullOrEmpty(TicketUrl);
    public bool IsUncategorized => CategoryId == 11; // Undefined category

    public static AdminEventViewModel FromEntity(Event eventEntity)
    {
        return new AdminEventViewModel
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Date = eventEntity.Date,
            City = eventEntity.City,
            Location = eventEntity.Location,
            Description = eventEntity.Description,
            ImageUrl = eventEntity.ImageUrl,
            TicketUrl = eventEntity.TicketUrl,
            IsFree = eventEntity.IsFree,
            Price = eventEntity.Price,
            IsFeatured = eventEntity.IsFeatured,
            CategoryName = eventEntity.Category?.Name,
            CategoryId = eventEntity.CategoryId,
            SubCategoryName = eventEntity.SubCategory?.Name,
            SubCategoryId = eventEntity.SubCategoryId,
            Status = eventEntity.Status,
            Tags = eventEntity.EventTags?.Select(et => et.Tag.Name).ToList() ?? new List<string>(),
            CreatedAt = eventEntity.CreatedAt,
            UpdatedAt = eventEntity.UpdatedAt,
            SourceUrl = eventEntity.SourceUrl
        };
    }

    public static List<AdminEventViewModel> FromEntities(IEnumerable<Event> events)
    {
        return events.Select(FromEntity).ToList();
    }
}

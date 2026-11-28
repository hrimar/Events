using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

public class Event
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; } // TODO: Make it nullable as ticketstation don't have date for some events

    public TimeSpan? StartTime { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    // English title and description for the /en/ pages; never contain Cyrillic letters. Null until the event is
    // translated or filled in the admin: the English page then shows Name and is not indexed (GS-REQ-010, 10.4).
    [MaxLength(200)]
    public string? NameEn { get; set; }

    [MaxLength(4000)]
    public string? DescriptionEn { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [MaxLength(500)]
    public string? TicketUrl { get; set; }

    public bool IsFree { get; set; }

    public decimal? Price { get; set; }

    public bool IsFeatured { get; set; } = false;

    [Required]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; }

    public int? CanonicalVenueId { get; set; }
    public CanonicalVenue? CanonicalVenue { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Published;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(500)]
    public string? SourceUrl { get; set; }

    // External address the image was copied from (Excel import); kept for reference only, never displayed.
    // Longer than ImageUrl because Facebook CDN addresses carry long signature parameters (GS-REQ-009, 9.1.5).
    [MaxLength(2000)]
    public string? ImageSourceUrl { get; set; }

    // OrganizerForm for events created from an approved "Add event" submission; Unknown otherwise (GS-REQ-011, 11.6.4).
    public EventSource Source { get; set; } = EventSource.Unknown;

    public ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();
}
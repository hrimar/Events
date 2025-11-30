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

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

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

    public EventStatus Status { get; set; } = EventStatus.Published;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? SourceUrl { get; set; }

    public ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();
}
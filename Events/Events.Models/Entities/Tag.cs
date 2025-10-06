using Events.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public EventCategory? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<EventTag> EventTags { get; set; } = new List<EventTag>();
}

using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

public class SubCategory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public EventCategory ParentCategory { get; set; }

    public int EnumValue { get; set; } // Maps to enum values 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;

namespace Events.Models.Entities;

// A "What" chip in the homepage hero (e.g. "Business" = categories Business and Conferences),
// managed from the admin "Site Content" screen so the owner can change the list without a deploy.
public class HomeQuickFilter
{
    // Upper limit of active chips shown in the hero.
    public const int MaxActive = 8;

    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string LabelBg { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LabelEn { get; set; } = string.Empty;

    public HomeQuickFilterType Type { get; set; }

    // Category names (EventCategory) or tag names, depending on Type; empty for Free.
    public List<string> Values { get; set; } = new();

    // Position in the hero, ascending.
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime UpdatedAt { get; set; }
}

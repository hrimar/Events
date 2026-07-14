using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

// Singleton row (Id = 1) holding editable Hero and About Us content managed from the admin panel.
public class SiteContent
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string HeroTitleBg { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string HeroTitleEn { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string HeroSubtitleBg { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string HeroSubtitleEn { get; set; } = string.Empty;

    // Sanitized HTML (bold, links, paragraphs only) - safe to render with Html.Raw.
    public string AboutUsContentBg { get; set; } = string.Empty;

    // Sanitized HTML (bold, links, paragraphs only) - safe to render with Html.Raw.
    public string AboutUsContentEn { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}

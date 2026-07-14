using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

// One row per static/category page SEO meta, keyed by PageKey (see SeoPageKeys) so new pages
// can be added later (sitemap, other static pages) without a schema change.
public class PageSeoMeta
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string PageKey { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TitleBg { get; set; }

    [MaxLength(200)]
    public string? TitleEn { get; set; }

    [MaxLength(300)]
    public string? DescriptionBg { get; set; }

    [MaxLength(300)]
    public string? DescriptionEn { get; set; }

    public DateTime UpdatedAt { get; set; }
}

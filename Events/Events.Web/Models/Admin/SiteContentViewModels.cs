using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.Admin;

public class SiteContentIndexViewModel
{
    public HeroFormViewModel Hero { get; set; } = new();
    public AboutUsFormViewModel AboutUs { get; set; } = new();
    public List<PageSeoMetaFormItem> SeoItems { get; set; } = new();
}

public class HeroFormViewModel
{
    [Required]
    [StringLength(200)]
    [Display(Name = "Hero Title (BG)")]
    public string HeroTitleBg { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Hero Title (EN)")]
    public string HeroTitleEn { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Hero Subtitle (BG)")]
    public string HeroSubtitleBg { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Display(Name = "Hero Subtitle (EN)")]
    public string HeroSubtitleEn { get; set; } = string.Empty;
}

public class AboutUsFormViewModel
{
    [Required]
    [Display(Name = "About Us Content (BG)")]
    public string AboutUsContentBg { get; set; } = string.Empty;

    [Required]
    [Display(Name = "About Us Content (EN)")]
    public string AboutUsContentEn { get; set; } = string.Empty;
}

public class PageSeoMetaFormItem
{
    [Required]
    public string PageKey { get; set; } = string.Empty;

    // Read-only label shown in the admin table, not persisted (e.g. "Home", "Category: Music").
    public string DisplayName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? TitleBg { get; set; }

    [StringLength(200)]
    public string? TitleEn { get; set; }

    [StringLength(300)]
    public string? DescriptionBg { get; set; }

    [StringLength(300)]
    public string? DescriptionEn { get; set; }
}

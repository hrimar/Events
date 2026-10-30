using System.ComponentModel.DataAnnotations;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Web.Models.Admin;

public class SiteContentIndexViewModel
{
    public HeroFormViewModel Hero { get; set; } = new();
    public AboutUsFormViewModel AboutUs { get; set; } = new();
    public List<PageSeoMetaFormItem> SeoItems { get; set; } = new();

    // Homepage "What" chips (GS-REQ-007, 7.3), in display order.
    public List<HomeQuickFilterListItem> QuickFilters { get; set; } = new();

    // Add form, or the edit form when a filter was picked for editing.
    public HomeQuickFilterFormViewModel QuickFilterForm { get; set; } = new();

    // Tab to open when the view is re-rendered after a failed POST (the query string has no "tab" then).
    public string? ActiveTab { get; set; }

    public int ActiveQuickFilterCount => QuickFilters.Count(f => f.IsActive);
}

public class HomeQuickFilterListItem
{
    public int Id { get; set; }
    public string LabelBg { get; set; } = string.Empty;
    public string LabelEn { get; set; } = string.Empty;
    public HomeQuickFilterType Type { get; set; }
    public List<string> Values { get; set; } = new();
    public bool IsActive { get; set; }

    public static HomeQuickFilterListItem FromEntity(HomeQuickFilter filter) => new()
    {
        Id = filter.Id,
        LabelBg = filter.LabelBg,
        LabelEn = filter.LabelEn,
        Type = filter.Type,
        Values = filter.Values,
        IsActive = filter.IsActive
    };
}

public class HomeQuickFilterFormViewModel
{
    // Null when adding a new filter.
    public int? Id { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Label (BG)")]
    public string LabelBg { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Label (EN)")]
    public string LabelEn { get; set; } = string.Empty;

    [Display(Name = "Type")]
    public HomeQuickFilterType Type { get; set; } = HomeQuickFilterType.Categories;

    // Used when Type is Categories (EventCategory names).
    public List<string> Categories { get; set; } = new();

    // Used when Type is Tags: comma-separated tag names as stored in the database (English, e.g. "For kids").
    [StringLength(400)]
    [Display(Name = "Tags (comma-separated)")]
    public string? Tags { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    // The values for the selected type, as the service expects them.
    public IReadOnlyList<string> GetValues() => Type switch
    {
        HomeQuickFilterType.Categories => Categories,
        HomeQuickFilterType.Tags => (Tags ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        _ => Array.Empty<string>()
    };

    public static HomeQuickFilterFormViewModel FromEntity(HomeQuickFilter filter) => new()
    {
        Id = filter.Id,
        LabelBg = filter.LabelBg,
        LabelEn = filter.LabelEn,
        Type = filter.Type,
        Categories = filter.Type == HomeQuickFilterType.Categories ? filter.Values.ToList() : new List<string>(),
        Tags = filter.Type == HomeQuickFilterType.Tags ? string.Join(", ", filter.Values) : null,
        IsActive = filter.IsActive
    };
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

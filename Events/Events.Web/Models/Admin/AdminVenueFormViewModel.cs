using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.Admin;

public class AdminVenueFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Full Name (BG)")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Full Name (EN)")]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Short Name")]
    public string? ShortName { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Slug")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Address")]
    public string? Address { get; set; }

    [StringLength(100)]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Range(-90, 90)]
    [Display(Name = "Latitude")]
    public decimal? Latitude { get; set; }

    [Range(-180, 180)]
    [Display(Name = "Longitude")]
    public decimal? Longitude { get; set; }

    [StringLength(2000)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [StringLength(500)]
    [Display(Name = "Photo URL")]
    public string? PhotoUrl { get; set; }

    [StringLength(500)]
    [Display(Name = "Website URL")]
    public string? WebsiteUrl { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Capacity")]
    public int? Capacity { get; set; }

    // Populated in Edit mode — shows aliases currently linked to this venue
    public IReadOnlyList<AdminVenueAliasViewModel> ExistingAliases { get; set; } =
        Array.Empty<AdminVenueAliasViewModel>();
}

public class AdminVenueAliasViewModel
{
    public int Id { get; set; }
    public string AliasString { get; set; } = string.Empty;
    public string NormalizedString { get; set; } = string.Empty;
}

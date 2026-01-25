using System.ComponentModel.DataAnnotations;
using Events.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models.Admin;

public class AdminTagFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Category")]
    public EventCategory? Category { get; set; }

    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = Enumerable.Empty<SelectListItem>();
}

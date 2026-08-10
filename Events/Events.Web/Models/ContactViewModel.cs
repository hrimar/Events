using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Models;

public class ContactViewModel
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public ContactTopic Topic { get; set; }

    [Required]
    [StringLength(3000, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;

    // Honeypot: hidden from sighted users via CSS rather than display:none/type=hidden.
    // Real visitors leave it empty. Avoid classic names like "Website" that sophisticated bots know to skip.
    public string? FaxNumber { get; set; }

    // Protected "form issued at" token used for minimum-fill-time checks.
    public string? FormIssuedAt { get; set; }

    public List<SelectListItem> TopicOptions { get; set; } = new();
}

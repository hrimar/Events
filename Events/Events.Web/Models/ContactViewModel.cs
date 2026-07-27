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

    // Honeypot field: legitimate visitors never fill it in, since it's hidden from sighted users
    // via CSS rather than display:none/type=hidden, which bots tend to skip.
    public string? Website { get; set; }

    public List<SelectListItem> TopicOptions { get; set; } = new();
}

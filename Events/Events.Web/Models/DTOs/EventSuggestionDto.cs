using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.DTOs;

public class EventSuggestionDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public string Date { get; set; } = string.Empty;
    
    public string[] Tags { get; set; } = Array.Empty<string>();
}
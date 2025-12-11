using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.DTOs;

public class SearchSuggestionDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    public string Date { get; set; } = string.Empty;
    
    public string[] Tags { get; set; } = Array.Empty<string>();
    
    public bool IsTag { get; set; } = false;
    
    public string? TagName { get; set; }
}
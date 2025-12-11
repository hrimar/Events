using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.DTOs;

public class TagSuggestionDto
{
    [Required]
    public int Id { get; set; } = 0;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Category { get; set; } = "Tag";
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    public string Date { get; set; } = string.Empty;
    
    public bool IsTag { get; set; } = true;
    
    [Required]
    public string TagName { get; set; } = string.Empty;
}
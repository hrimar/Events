using Events.Models.Enums;

namespace Events.Services.Models.Admin.DTOs;

public class AdminTagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventCategory? Category { get; set; }
    public int UsageCount { get; set; }
    public IReadOnlyList<string> CategoryUsage { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> SubCategoryUsage { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; }
}

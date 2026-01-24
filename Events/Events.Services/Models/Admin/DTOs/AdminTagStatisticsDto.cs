namespace Events.Services.Models.Admin.DTOs;

public class AdminTagStatisticsDto
{
    public int TotalTags { get; set; }
    public int OrphanTags { get; set; }
    public int WithoutCategoryTags { get; set; }
    public string? MostUsedTagName { get; set; }
    public int MostUsedTagCount { get; set; }
}

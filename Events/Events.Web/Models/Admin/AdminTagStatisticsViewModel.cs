namespace Events.Web.Models.Admin;

public class AdminTagStatisticsViewModel
{
    public int TotalTags { get; set; }
    public int OrphanTags { get; set; }
    public int WithoutCategoryTags { get; set; }
    public string? MostUsedTagName { get; set; }
    public int MostUsedTagCount { get; set; }
}

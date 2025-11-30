namespace Events.Web.Models.Admin;

public class UncategorizedEventsViewModel
{
    public PaginatedList<AdminEventViewModel> Events { get; set; } = new(new List<AdminEventViewModel>(), 0, 1, 20);
    public List<CategoryOption> AvailableCategories { get; set; } = new();
    public Dictionary<int, List<SubCategoryOption>> AvailableSubCategories { get; set; } = new();

    public string? SearchTerm { get; set; }
}

public class CategoryOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SubCategoryOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}

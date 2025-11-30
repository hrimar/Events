using Events.Models.Enums;

namespace Events.Web.Models.Admin;

public class EditEventViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public string City { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }
    public bool IsFree { get; set; }
    public decimal? Price { get; set; }
    public bool IsFeatured { get; set; }
    public int CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public EventStatus Status { get; set; }
    public string? SourceUrl { get; set; }

    public List<CategoryOption> AvailableCategories { get; set; } = new();
    public List<SubCategoryOption> AvailableSubCategories { get; set; } = new();
}

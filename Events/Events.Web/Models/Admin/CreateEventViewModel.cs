using Events.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Events.Web.Models.Admin;

public class CreateEventViewModel
{
    [Required(ErrorMessage = "Event name is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location/Venue is required")]
    [StringLength(300, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 300 characters")]
    public string Location { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Please enter a valid image URL")]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ticket URL cannot exceed 500 characters")]
    [Url(ErrorMessage = "Please enter a valid ticket URL")]
    public string? TicketUrl { get; set; }

    public bool IsFree { get; set; }

    [Range(0, 1000, ErrorMessage = "Price must be between 0 and 1000")]
    public decimal? Price { get; set; }

    public bool IsFeatured { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Subcategory is required")]
    public int? SubCategoryId { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Published;

    // Tags - maximum 3
    [MaxLength(3, ErrorMessage = "You can select a maximum of 3 tags")]
    public List<int> SelectedTagIds { get; set; } = new();

    public List<CategoryOption> AvailableCategories { get; set; } = new();
    public List<SubCategoryOption> AvailableSubCategories { get; set; } = new();
    public List<TagOption> AvailableTags { get; set; } = new();
}

public class TagOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

namespace Events.Web.Models.DTOs;

/// <summary>
/// DTO for SubCategory in API responses.
/// </summary>
public class SubCategoryDto
{
    /// <summary>
    /// The ID of the subcategory.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the subcategory.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

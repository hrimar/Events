namespace Events.Web.Models.DTOs;

public class CategoryRedirectDto
{
    public string Category { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
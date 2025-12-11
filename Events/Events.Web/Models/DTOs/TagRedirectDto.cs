namespace Events.Web.Models.DTOs;

public class TagRedirectDto
{
    public string Tags { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
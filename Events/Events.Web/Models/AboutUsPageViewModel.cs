namespace Events.Web.Models;

public class AboutUsPageViewModel
{
    // Sanitized HTML from Site Content admin - safe to render with Html.Raw.
    public string Content { get; set; } = string.Empty;
}

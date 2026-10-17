namespace Events.Web.Models.DTOs;

// Header search suggestions (autocomplete), grouped as shown in the dropdown.
public sealed class SearchSuggestionsDto
{
    public IReadOnlyList<SearchSuggestionDto> Events { get; init; } = Array.Empty<SearchSuggestionDto>();
    public IReadOnlyList<SearchSuggestionDto> Venues { get; init; } = Array.Empty<SearchSuggestionDto>();
}

// One suggestion row. All texts are already localized and formatted on the server.
public sealed class SearchSuggestionDto
{
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;

    // Secondary line: "date · time · venue" for events, address for venues.
    public string? Details { get; init; }

    // Localized category name (events only).
    public string? Category { get; init; }
}

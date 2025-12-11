using Events.Web.Models;
using Events.Web.Models.DTOs;

namespace Events.Web.Extensions;

public static class TagExtensions
{
    public static TagSuggestionDto ToSuggestionDto(this TagViewModel tagViewModel)
    {
        return new TagSuggestionDto
        {
            Id = 0,
            Name = "#" + tagViewModel.Name,
            Category = "Tag",
            Location = $"{tagViewModel.EventCount} events",
            Date = string.Empty,
            IsTag = true,
            TagName = tagViewModel.Name
        };
    }

    public static SearchSuggestionDto ToSearchSuggestionDto(this TagViewModel tagViewModel)
    {
        return new SearchSuggestionDto
        {
            Id = 0,
            Name = "#" + tagViewModel.Name,
            Category = "Tag",
            Location = $"{tagViewModel.EventCount} events",
            Date = string.Empty,
            Tags = Array.Empty<string>(),
            IsTag = true,
            TagName = tagViewModel.Name
        };
    }
}
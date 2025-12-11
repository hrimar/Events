using Events.Models.Entities;
using Events.Web.Models.DTOs;

namespace Events.Web.Extensions;

public static class EventExtensions
{
    public static EventSuggestionDto ToSuggestionDto(this Event eventEntity)
    {
        var tags = eventEntity.EventTags?
            .Take(3)
            .Select(et => et.Tag?.Name)
            .OfType<string>()
            .ToArray() ?? Array.Empty<string>();

        return new EventSuggestionDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Category = eventEntity.Category?.Name,
            Location = eventEntity.Location,
            Date = eventEntity.Date.ToString("dd.MM.yyyy"),
            Tags = tags
        };
    }

    public static SearchSuggestionDto ToSearchSuggestionDto(this Event eventEntity)
    {
        var tags = eventEntity.EventTags?
            .Take(3)
            .Select(et => et.Tag?.Name)
            .OfType<string>()
            .ToArray() ?? Array.Empty<string>();

        return new SearchSuggestionDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Category = eventEntity.Category?.Name,
            Location = eventEntity.Location,
            Date = eventEntity.Date.ToString("dd.MM.yyyy"),
            Tags = tags,
            IsTag = false,
            TagName = null
        };
    }
}
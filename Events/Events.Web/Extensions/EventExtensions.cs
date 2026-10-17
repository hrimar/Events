using System.Globalization;
using Events.Models.Entities;
using Events.Web.Infrastructure.JsonLd;
using Events.Web.Localization;
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

    // Autocomplete row: "date · time · venue"; the time is omitted when unknown (stored as 00:00).
    public static SearchSuggestionDto ToSearchSuggestionDto(this Event eventEntity, bool isEnglish, string? localizedCategory)
    {
        var date = eventEntity.Date.ToString(isEnglish ? "dd/MM/yyyy" : "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var time = EventDateTimeHelper.HasKnownTime(eventEntity.Date, eventEntity.StartTime)
            ? (eventEntity.StartTime ?? eventEntity.Date.TimeOfDay).ToString(@"hh\:mm", CultureInfo.InvariantCulture)
            : null;

        var venue = eventEntity.CanonicalVenue?.LocalizedName(isEnglish) ?? eventEntity.Location;

        var details = string.Join(" · ", new[] { date, time, venue }.Where(part => !string.IsNullOrWhiteSpace(part)));

        return new SearchSuggestionDto
        {
            Title = eventEntity.Name,
            Url = $"/Events/Details/{eventEntity.Id}",
            Details = details,
            Category = localizedCategory
        };
    }
}
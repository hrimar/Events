using Events.Models.Entities;
using Events.Web.Localization;
using Events.Web.Models.DTOs;

namespace Events.Web.Extensions;

public static class VenueExtensions
{
    public static SearchSuggestionDto ToSearchSuggestionDto(this CanonicalVenue venue, bool isEnglish)
    {
        return new SearchSuggestionDto
        {
            Title = venue.LocalizedName(isEnglish),
            Url = $"/venues/{venue.Slug}",
            Details = venue.Address
        };
    }
}

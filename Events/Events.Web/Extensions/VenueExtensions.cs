using Events.Models.Entities;
using Events.Web.Localization;
using Events.Web.Models.DTOs;

namespace Events.Web.Extensions;

public static class VenueExtensions
{
    // urlPrefix is the request's PathBase ("/en" on English pages, GS-REQ-010, 10.2.5).
    public static SearchSuggestionDto ToSearchSuggestionDto(this CanonicalVenue venue, bool isEnglish, string urlPrefix = "")
    {
        return new SearchSuggestionDto
        {
            Title = venue.LocalizedName(isEnglish),
            Url = $"{urlPrefix}/venues/{venue.Slug}",
            Details = venue.Address
        };
    }
}

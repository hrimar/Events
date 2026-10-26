using Events.Models.Entities;
using Events.Models.DTOs;

namespace Events.Data.Repositories.Interfaces;

public interface IVenueRepository
{
    Task<CanonicalVenue?> GetByIdAsync(int id);
    Task<CanonicalVenue?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug);
    Task<IEnumerable<CanonicalVenue>> GetAllAsync();
    Task<CanonicalVenue> AddAsync(CanonicalVenue venue);
    Task<CanonicalVenue> UpdateAsync(CanonicalVenue venue);
    Task DeleteAsync(int id);

    // Looks up a canonical venue by a normalized alias string
    Task<CanonicalVenue?> FindByNormalizedAliasAsync(string normalizedAlias);

    Task<bool> AliasExistsAsync(string normalizedAlias);
    Task<VenueAlias> AddAliasAsync(VenueAlias alias);
    Task DeleteAliasAsync(int aliasId);

    // Returns venues with precomputed alias count and event counts for admin list.
    // fromDate is the calendar day from which events count as upcoming (Europe/Sofia "today").
    Task<IEnumerable<VenueWithStatsDto>> GetAllWithStatsAsync(DateTime fromDate);

    // Venues with a photo and the most published events between fromDate and toDate (calendar days, inclusive),
    // most first; UpcomingEventsCount is the count in that period (homepage "Venues", GS-REQ-007, 7.10.2).
    Task<List<VenueWithStatsDto>> GetTopVenuesWithPhotoAsync(DateTime fromDate, DateTime toDate, int count);

    // Number of venues in the catalog.
    Task<int> CountAsync();

    // Returns distinct unmapped location strings grouped by count, ordered by EventCount desc
    Task<IEnumerable<UnmappedLocationDto>> GetUnmappedLocationsAsync();

    // Returns upcoming published events for a given venue, ordered by date
    Task<IEnumerable<Event>> GetUpcomingEventsByVenueAsync(int venueId, DateTime fromDate);

    // Returns venues whose BG name, EN name or short name contains the search term (case-insensitive),
    // names starting with the term first.
    Task<IEnumerable<CanonicalVenue>> SearchByNameAsync(string searchTerm, int maxCount);
}

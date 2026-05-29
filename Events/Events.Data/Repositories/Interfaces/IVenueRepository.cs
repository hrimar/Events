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

    // Returns venues with precomputed alias count and event counts for admin list
    Task<IEnumerable<VenueWithStatsDto>> GetAllWithStatsAsync();

    // Returns distinct unmapped location strings grouped by count, ordered by EventCount desc
    Task<IEnumerable<UnmappedLocationDto>> GetUnmappedLocationsAsync();

    // Returns upcoming published events for a given venue, ordered by date
    Task<IEnumerable<Event>> GetUpcomingEventsByVenueAsync(int venueId);
}

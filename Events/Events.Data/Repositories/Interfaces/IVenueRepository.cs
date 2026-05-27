using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface IVenueRepository
{
    Task<CanonicalVenue?> GetByIdAsync(int id);
    Task<CanonicalVenue?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug);
    Task<IEnumerable<CanonicalVenue>> GetAllAsync();
    Task<CanonicalVenue> AddAsync(CanonicalVenue venue);
    Task<CanonicalVenue> UpdateAsync(CanonicalVenue venue);

    // Looks up a canonical venue by a normalized alias string
    Task<CanonicalVenue?> FindByNormalizedAliasAsync(string normalizedAlias);

    Task<bool> AliasExistsAsync(string normalizedAlias);
    Task<VenueAlias> AddAliasAsync(VenueAlias alias);
}

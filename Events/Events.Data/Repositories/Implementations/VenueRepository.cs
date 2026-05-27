using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Repositories.Implementations;

public class VenueRepository : IVenueRepository
{
    private readonly EventsDbContext _context;

    public VenueRepository(EventsDbContext context)
    {
        _context = context;
    }

    public async Task<CanonicalVenue?> GetByIdAsync(int id)
    {
        return await _context.CanonicalVenues
            .Include(v => v.Aliases)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<CanonicalVenue?> GetBySlugAsync(string slug)
    {
        return await _context.CanonicalVenues
            .Include(v => v.Aliases)
            .FirstOrDefaultAsync(v => v.Slug == slug);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _context.CanonicalVenues.AnyAsync(v => v.Slug == slug);
    }

    public async Task<IEnumerable<CanonicalVenue>> GetAllAsync()
    {
        return await _context.CanonicalVenues
            .OrderBy(v => v.Name)
            .ToListAsync();
    }

    public async Task<CanonicalVenue> AddAsync(CanonicalVenue venue)
    {
        _context.CanonicalVenues.Add(venue);
        await _context.SaveChangesAsync();
        return venue;
    }

    public async Task<CanonicalVenue> UpdateAsync(CanonicalVenue venue)
    {
        _context.CanonicalVenues.Update(venue);
        await _context.SaveChangesAsync();
        return venue;
    }

    public async Task<CanonicalVenue?> FindByNormalizedAliasAsync(string normalizedAlias)
    {
        return await _context.VenueAliases
            .Where(a => a.NormalizedString == normalizedAlias)
            .Select(a => a.CanonicalVenue)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AliasExistsAsync(string normalizedAlias)
    {
        return await _context.VenueAliases
            .AnyAsync(a => a.NormalizedString == normalizedAlias);
    }

    public async Task<VenueAlias> AddAliasAsync(VenueAlias alias)
    {
        _context.VenueAliases.Add(alias);
        await _context.SaveChangesAsync();
        return alias;
    }
}

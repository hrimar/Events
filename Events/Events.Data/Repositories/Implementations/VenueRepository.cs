using Events.Data.Context;
using Events.Data.Repositories.Interfaces;
using Events.Models.DTOs;
using Events.Models.Entities;
using Events.Models.Enums;
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

    public async Task DeleteAsync(int id)
    {
        var venue = await _context.CanonicalVenues.FindAsync(id);
        if (venue == null)
            throw new KeyNotFoundException($"Venue with ID {id} was not found.");

        _context.CanonicalVenues.Remove(venue);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAliasAsync(int aliasId)
    {
        var alias = await _context.VenueAliases.FindAsync(aliasId);
        if (alias == null)
            throw new KeyNotFoundException($"Venue alias with ID {aliasId} was not found.");

        _context.VenueAliases.Remove(alias);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<VenueWithStatsDto>> GetAllWithStatsAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.CanonicalVenues
            .Select(v => new VenueWithStatsDto
            {
                Id = v.Id,
                Name = v.Name,
                NameEn = v.NameEn,
                ShortName = v.ShortName,
                Slug = v.Slug,
                City = v.City,
                AliasCount = v.Aliases.Count,
                UpcomingEventsCount = v.Events.Count(e => e.Date >= now && e.Status == EventStatus.Published),
                TotalEventsCount = v.Events.Count()
            })
            .OrderBy(v => v.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<UnmappedLocationDto>> GetUnmappedLocationsAsync()
    {
        return await _context.Events
            .Where(e => e.CanonicalVenueId == null && e.Location != string.Empty)
            .GroupBy(e => e.Location)
            .Select(g => new UnmappedLocationDto
            {
                Location = g.Key,
                EventCount = g.Count()
            })
            .OrderByDescending(x => x.EventCount)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsByVenueAsync(int venueId)
    {
        var now = DateTime.UtcNow;

        return await _context.Events
            .Include(e => e.Category)
            .Include(e => e.SubCategory)
            .Include(e => e.CanonicalVenue)
            .Include(e => e.EventTags)
            .ThenInclude(et => et.Tag)
            .Where(e => e.CanonicalVenueId == venueId && e.Status == EventStatus.Published && e.Date >= now)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }
}

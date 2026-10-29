using Events.Models.DTOs;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;

namespace Events.Data.Repositories.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null);
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Event>> GetByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetByCategoryIdAsync(int categoryId);
    Task<IEnumerable<Event>> GetByLocationAsync(string location);
    Task<IEnumerable<Event>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Event> AddAsync(Event eventEntity);
    Task<Event> UpdateAsync(Event eventEntity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<Event?> FindByNameAsync(string name);
    Task<IEnumerable<Event>> FindAllByNameAsync(string name);

    // pagination and filtering
    Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        PublicEventQuery eventQuery,
        CancellationToken cancellationToken = default);

    // Id, date, time and name of the events matching the public filters (paging and sorting are ignored),
    // at most maxCount, for grouping a period by day (GS-REQ-008, 8.2.6).
    Task<List<EventListingKeyDto>> GetListingKeysAsync(PublicEventQuery eventQuery, int maxCount,
        CancellationToken cancellationToken = default);

    // Full events (category, subcategory, venue, tags) for the given ids, in no particular order.
    Task<List<Event>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsAsync(EventListCriteria criteria);

    // fromDate is the calendar day from which events count as upcoming (Europe/Sofia "today").
    Task<IEnumerable<Event>> GetFeaturedEventsAsync(DateTime fromDate, int count = 10);

    // The categories with the most published events between fromDate and toDate (calendar days, inclusive),
    // most first, with the image of each category's nearest upcoming event; excludedImageUrl (the site's
    // placeholder) does not count as an image. The Undefined category is left out.
    Task<List<CategoryUpcomingSummaryDto>> GetTopUpcomingCategoriesAsync(DateTime fromDate, DateTime toDate, int count, string excludedImageUrl);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(DateTime fromDate, int count = 10);
    Task<int> GetTotalEventsCountAsync(EventStatus? status = null);

    /// <summary>
    /// Batch update multiple events with a single SaveChanges call for optimal performance.
    /// </summary>
    Task<int> BulkUpdateAsync(IEnumerable<Event> events);

    /// <summary>
    /// Updates CanonicalVenueId for all events matching the given raw location string.
    /// Used by admin when mapping an unmapped location to a canonical venue.
    /// Returns the number of events updated.
    /// </summary>
    Task<int> UpdateCanonicalVenueByLocationAsync(string location, int canonicalVenueId);
}
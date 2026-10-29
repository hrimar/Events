using Events.Models.DTOs;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;

namespace Events.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetEventByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<int> GetEventsCountInRangeAsync(DateTime fromDate, DateTime toDate, EventStatus? status = null);
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);

    // Public listings (all events, categories, search, related events, sitemap).
    // Results are cached for a few minutes, except search queries.
    Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        PublicEventQuery eventQuery,
        CancellationToken cancellationToken = default);

    // Id, date, time and name of every event matching the public filters (at most maxCount), for grouping
    // a period by day (GS-REQ-008, 8.2.6). Cached like GetPagedEventsAsync.
    Task<List<EventListingKeyDto>> GetListingKeysAsync(PublicEventQuery eventQuery, int maxCount,
        CancellationToken cancellationToken = default);

    // Full events for the given ids (the cards of a period view), in no particular order. Cached.
    Task<List<Event>> GetEventsByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);

    Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsAsync(EventListCriteria criteria);

    Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10);

    // The categories with the most published events from today on for the given number of days (GS-REQ-007, 7.8.2).
    Task<List<CategoryUpcomingSummaryDto>> GetTopUpcomingCategoriesAsync(int days, int count, string excludedImageUrl);
    Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10);
    // Unfiltered search over all events (any status and date) - for the admin panel only.
    // Public search goes through GetPagedEventsAsync with PublicEventQuery.Search.
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category);
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<int> GetTotalEventsCountAsync(EventStatus? status = null);
    Task<bool> EventExistsAsync(int id);
    Task<Event?> FindEventByNameAsync(string name);
    Task<IEnumerable<Event>> FindEventsByNameAsync(string name);

    /// <summary>
    /// Batch update multiple events efficiently in a single database transaction.
    /// 
    /// Advantages of batch operations:
    /// - Single database round-trip (not N round-trips for N events)
    /// - One transaction scope ensures data consistency
    /// - Optimal for admin operations affecting 5-100 events
    /// - Better performance than sequential individual updates
    /// </summary>
    Task<int> BulkUpdateEventsAsync(IEnumerable<Event> events);
}
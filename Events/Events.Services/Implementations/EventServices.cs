using Events.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        try
        {
            return await _eventRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event with ID {EventId}", id);
            throw new ApplicationException($"Failed to retrieve event with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        try
        {
            return await _eventRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all events");
            throw new ApplicationException("Failed to retrieve events", ex);
        }
    }

    public async Task<(IEnumerable<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int page,
        int pageSize,
        EventStatus? status = null,
        string? categoryName = null,
        bool? isFree = null,
        DateTime? fromDate = null)
    {
        try
        {
            if (page < 1) page = 1;
            
            // Allow large pageSize for "get all" scenarios (Controller uses int.MaxValue)
            if (pageSize < 1) pageSize = 12;
            if (pageSize > 50000) pageSize = 50000; // Safety limit to prevent memory issues

            _logger.LogInformation("Getting paged events: Page {Page}, PageSize {PageSize}, Status {Status}, Category {Category}, FromDate {FromDate}",
                page, pageSize, status, categoryName, fromDate);

            return await _eventRepository.GetPagedEventsAsync(page, pageSize, status, categoryName, isFree, fromDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged events");
            throw new ApplicationException("Failed to retrieve paged events", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 10)
    {
        try
        {
            count = Math.Min(count, 50);
            return await _eventRepository.GetFeaturedEventsAsync(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured events");
            throw new ApplicationException("Failed to retrieve featured events", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int count = 10)
    {
        try
        {
            count = Math.Min(count, 100);
            return await _eventRepository.GetUpcomingEventsAsync(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming events");
            throw new ApplicationException("Failed to retrieve upcoming events", ex);
        }
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Event>();
            }

            searchTerm = searchTerm.Trim();

            return await _eventRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events with term {SearchTerm}", searchTerm);
            throw new ApplicationException($"Failed to search events with term '{searchTerm}'", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(EventCategory category)
    {
        try
        {
            return await _eventRepository.GetByCategoryAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by category {Category}", category);
            throw new ApplicationException($"Failed to retrieve events for category {category}", ex);
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date cannot be after end date");
            }

            return await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by date range {StartDate} - {EndDate}", startDate, endDate);
            throw new ApplicationException($"Failed to retrieve events for date range {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}", ex);
        }
    }

    public async Task<Event> CreateEventAsync(Event eventEntity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(eventEntity.Name))
            {
                throw new ArgumentException("Event name is required");
            }

            return await _eventRepository.AddAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event {EventName}", eventEntity.Name);
            throw new ApplicationException($"Failed to create event '{eventEntity.Name}'", ex);
        }
    }

    public async Task<Event> UpdateEventAsync(Event eventEntity)
    {
        try
        {
            if (!await _eventRepository.ExistsAsync(eventEntity.Id))
            {
                throw new InvalidOperationException($"Event with ID {eventEntity.Id} not found");
            }

            return await _eventRepository.UpdateAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", eventEntity.Id);
            throw new ApplicationException($"Failed to update event with ID {eventEntity.Id}", ex);
        }
    }

    public async Task DeleteEventAsync(int id)
    {
        try
        {
            if (!await _eventRepository.ExistsAsync(id))
            {
                throw new InvalidOperationException($"Event with ID {id} not found");
            }

            await _eventRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw new ApplicationException($"Failed to delete event with ID {id}", ex);
        }
    }

    public async Task<int> GetTotalEventsCountAsync(EventStatus? status = null)
    {
        try
        {
            return await _eventRepository.GetTotalEventsCountAsync(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total events count");
            throw new ApplicationException("Failed to get events count", ex);
        }
    }

    public async Task<bool> EventExistsAsync(int id)
    {
        try
        {
            return await _eventRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if event exists {EventId}", id);
            return false;
        }
    }
}
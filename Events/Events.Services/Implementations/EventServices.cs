using Events.Services.Interfaces;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
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
            throw;
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
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by date range {StartDate} - {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(Events.Models.Enums.EventCategory category)
    {
        try
        {
            return await _eventRepository.GetByCategoryAsync(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting events by category {Category}", category);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        try
        {
            return await _eventRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching events with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<Event> CreateEventAsync(Event eventEntity)
    {
        try
        {
            return await _eventRepository.AddAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event {EventName}", eventEntity.Name);
            throw;
        }
    }

    public async Task<Event> UpdateEventAsync(Event eventEntity)
    {
        try
        {
            return await _eventRepository.UpdateAsync(eventEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId}", eventEntity.Id);
            throw;
        }
    }

    public async Task DeleteEventAsync(int id)
    {
        try
        {
            await _eventRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw;
        }
    }
}

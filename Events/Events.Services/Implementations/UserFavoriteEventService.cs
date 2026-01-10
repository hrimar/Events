using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

/// <summary>
/// Service implementation for managing user favorite events.
/// </summary>
public class UserFavoriteEventService : IUserFavoriteEventService
{
    private readonly IUserFavoriteEventRepository _repository;
    private readonly ILogger<UserFavoriteEventService> _logger;

    public UserFavoriteEventService(IUserFavoriteEventRepository repository, ILogger<UserFavoriteEventService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> ToggleFavoriteAsync(string userId, int eventId)
    {
        try
        {
            var isFavorite = await _repository.IsFavoriteAsync(userId, eventId);

            if (isFavorite)
            {
                await _repository.RemoveFavoriteAsync(userId, eventId);
                _logger.LogInformation("User {UserId} removed event {EventId} from favorites", userId, eventId);
                return false;
            }
            else
            {
                await _repository.AddFavoriteAsync(userId, eventId);
                _logger.LogInformation("User {UserId} added event {EventId} to favorites", userId, eventId);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for user {UserId} and event {EventId}", userId, eventId);
            throw;
        }
    }

    public async Task<bool> AddFavoriteAsync(string userId, int eventId)
    {
        try
        {
            var result = await _repository.AddFavoriteAsync(userId, eventId);
            
            if (result != null)
            {
                _logger.LogInformation("User {UserId} added event {EventId} to favorites", userId, eventId);
                return true;
            }

            _logger.LogDebug("Event {EventId} was already in favorites for user {UserId}", eventId, userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite for user {UserId} and event {EventId}", userId, eventId);
            throw;
        }
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int eventId)
    {
        try
        {
            var result = await _repository.RemoveFavoriteAsync(userId, eventId);
            
            if (result)
            {
                _logger.LogInformation("User {UserId} removed event {EventId} from favorites", userId, eventId);
                return true;
            }

            _logger.LogDebug("Event {EventId} was not in favorites for user {UserId}", eventId, userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing favorite for user {UserId} and event {EventId}", userId, eventId);
            throw;
        }
    }

    public async Task<bool> IsFavoriteAsync(string userId, int eventId)
    {
        try
        {
            return await _repository.IsFavoriteAsync(userId, eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status for user {UserId} and event {EventId}", userId, eventId);
            throw;
        }
    }

    public async Task<IEnumerable<UserFavoriteEvent>> GetUserFavoritesAsync(string userId)
    {
        try
        {
            return await _repository.GetUserFavoritesAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving favorites for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetFavoriteCountAsync(string userId)
    {
        try
        {
            return await _repository.GetFavoriteCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorite count for user {UserId}", userId);
            throw;
        }
    }
}

using Events.Services.Interfaces;
using Events.Web.Models.DTOs;
using Events.Web.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace Events.Web.Controllers.Api;

/// <summary>
/// API controller for managing user favorite events.
/// Provides endpoints for toggling, adding, and removing favorite events.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IUserFavoriteEventService _favoriteService;
    private readonly ILogger<FavoritesController> _logger;
    private readonly IStringLocalizer<SharedResources> _localizer;

    public FavoritesController(
        IUserFavoriteEventService favoriteService,
        ILogger<FavoritesController> logger,
        IStringLocalizer<SharedResources> localizer)
    {
        _favoriteService = favoriteService;
        _logger = logger;
        _localizer = localizer;
    }

    /// <summary>
    /// Toggles the favorite status of an event for the current user.
    /// If the event is already favorited, it will be removed.
    /// If it's not favorited, it will be added.
    /// </summary>
    /// <param name="eventId">The ID of the event to toggle.</param>
    /// <returns>The new favorite status (true = favorited, false = removed).</returns>
    [HttpPost("toggle/{eventId}")]
    [ProducesResponseType(typeof(ToggleFavoriteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ToggleFavoriteDto>> ToggleFavorite(int eventId)
    {
        try
        {
            // Get current user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID could not be retrieved from claims");
                return Unauthorized("User ID could not be retrieved");
            }

            if (eventId <= 0)
            {
                _logger.LogWarning("Invalid event ID: {EventId}", eventId);
                return BadRequest("Event ID must be greater than 0");
            }

            // Toggle the favorite status
            var isFavorited = await _favoriteService.ToggleFavoriteAsync(userId, eventId);

            _logger.LogInformation("User {UserId} toggled favorite for event {EventId}. New status: {IsFavorited}", userId, eventId, isFavorited);

            return Ok(new ToggleFavoriteDto
            {
                EventId = eventId,
                IsFavorited = isFavorited,
                Message = isFavorited ? _localizer["Favorites_Added"] : _localizer["Favorites_Removed"]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for event {EventId}", eventId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer["Favorites_Error"].Value });
        }
    }

    /// <summary>
    /// Checks if an event is in the current user's favorites.
    /// </summary>
    /// <param name="eventId">The ID of the event to check.</param>
    /// <returns>The favorite status of the event.</returns>
    [HttpGet("check/{eventId}")]
    [ProducesResponseType(typeof(CheckFavoriteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CheckFavoriteDto>> CheckFavorite(int eventId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID could not be retrieved from claims");
                return Unauthorized("User ID could not be retrieved");
            }

            if (eventId <= 0)
            {
                _logger.LogWarning("Invalid event ID: {EventId}", eventId);
                return BadRequest("Event ID must be greater than 0");
            }

            var isFavorited = await _favoriteService.IsFavoriteAsync(userId, eventId);

            return Ok(new CheckFavoriteDto
            {
                EventId = eventId,
                IsFavorited = isFavorited
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite for event {EventId}", eventId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer["Favorites_Error"].Value });
        }
    }

    /// <summary>
    /// Gets the count of favorite events for the current user.
    /// </summary>
    /// <returns>The count of favorite events.</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(FavoriteCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FavoriteCountDto>> GetFavoriteCount()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID could not be retrieved from claims");
                return Unauthorized("User ID could not be retrieved");
            }

            var count = await _favoriteService.GetFavoriteCountAsync(userId);

            return Ok(new FavoriteCountDto
            {
                Count = count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorite count");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = _localizer["Favorites_Error"].Value });
        }
    }
}
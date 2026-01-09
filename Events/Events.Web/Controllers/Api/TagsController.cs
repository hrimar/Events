using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Events.Models.Enums;

namespace Events.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly IEventService _eventService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(ITagService tagService, IEventService eventService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _eventService = eventService;
        _logger = logger;
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<TagViewModel>>> GetPopularTags([FromQuery] int count = 20)
    {
        try
        {
            var futureEvents = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, null, null, DateTime.Today);
            
            var futureEventIds = futureEvents.Events.Select(e => e.Id).ToList();
            var allTags = await _tagService.GetAllTagsAsync();
            
            var popularTags = allTags
                .Where(t => t.EventTags?.Any(et => futureEventIds.Contains(et.EventId)) == true)
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    EventCount = t.EventTags?.Count(et => futureEventIds.Contains(et.EventId)) ?? 0,
                    Category = t.Category
                })
                .Where(t => t.EventCount > 0)
                .OrderByDescending(t => t.EventCount)
                .Take(count)
                .ToList();

            return Ok(popularTags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            return StatusCode(500, "Error getting popular tags");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<TagViewModel>>> SearchTags([FromQuery] string query, [FromQuery] int count = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Ok(new List<TagViewModel>());
            }

            var futureEvents = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, null, null, DateTime.Today);
            
            var futureEventIds = futureEvents.Events.Select(e => e.Id).ToList();
            var allTags = await _tagService.GetAllTagsAsync();
            
            var matchingTags = allTags
                .Where(t => t.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Where(t => t.EventTags?.Any(et => futureEventIds.Contains(et.EventId)) == true)
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    EventCount = t.EventTags?.Count(et => futureEventIds.Contains(et.EventId)) ?? 0,
                    Category = t.Category
                })
                .Where(t => t.EventCount > 0)
                .OrderByDescending(t => t.EventCount)
                .Take(count)
                .ToList();

            return Ok(matchingTags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags for query: {Query}", query);
            return StatusCode(500, "Error searching tags");
        }
    }

    [HttpGet("by-category/{category}")]
    public async Task<ActionResult<List<TagViewModel>>> GetTagsByCategory(string category, [FromQuery] int count = 20)
    {
        try
        {
            if (!Enum.TryParse<Events.Models.Enums.EventCategory>(category, true, out var categoryEnum))
            {
                return BadRequest("Invalid category");
            }

            var futureEvents = await _eventService.GetPagedEventsAsync(1, int.MaxValue, EventStatus.Published, null, null, DateTime.Today);
            
            var futureEventIds = futureEvents.Events.Select(e => e.Id).ToList();
            var allTags = await _tagService.GetAllTagsAsync();
            
            var categoryTags = allTags
                .Where(t => t.Category == categoryEnum)
                .Where(t => t.EventTags?.Any(et => futureEventIds.Contains(et.EventId)) == true)
                .Select(t => new TagViewModel
                {
                    Name = t.Name,
                    EventCount = t.EventTags?.Count(et => futureEventIds.Contains(et.EventId)) ?? 0,
                    Category = t.Category
                })
                .Where(t => t.EventCount > 0)
                .OrderByDescending(t => t.EventCount)
                .Take(count)
                .ToList();

            return Ok(categoryTags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags for category: {Category}", category);
            return StatusCode(500, "Error getting category tags");
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<object>>> GetAllTags()
    {
        try
        {
            var allTags = await _tagService.GetAllTagsAsync();
            var tags = allTags
                .Select(t => new { id = t.Id, name = t.Name })
                .OrderBy(t => t.name)
                .ToList();

            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tags");
            return StatusCode(500, "Error getting all tags");
        }
    }
}
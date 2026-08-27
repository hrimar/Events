using Events.Services.Interfaces;
using Events.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Events.Models.Enums;

namespace Events.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("events")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagsController> _logger;

    public TagsController(ITagService tagService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<TagViewModel>>> GetPopularTags([FromQuery] int count = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var popularTags = await _tagService.GetPopularTagsAsync(DateTime.Today, maxCount: count, cancellationToken: cancellationToken);

            return Ok(popularTags
                .Select(t => new TagViewModel { Name = t.Name, EventCount = t.EventCount, Category = t.Category })
                .ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular tags");
            return StatusCode(500, "Error getting popular tags");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<TagViewModel>>> SearchTags([FromQuery] string query, [FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Ok(new List<TagViewModel>());
            }

            var matchingTags = await _tagService.GetPopularTagsAsync(DateTime.Today, nameFilter: query, maxCount: count, cancellationToken: cancellationToken);

            return Ok(matchingTags
                .Select(t => new TagViewModel { Name = t.Name, EventCount = t.EventCount, Category = t.Category })
                .ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags for query: {Query}", query);
            return StatusCode(500, "Error searching tags");
        }
    }

    [HttpGet("by-category/{category}")]
    public async Task<ActionResult<List<TagViewModel>>> GetTagsByCategory(string category, [FromQuery] int count = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Enum.TryParse<EventCategory>(category, true, out var categoryEnum))
            {
                return BadRequest("Invalid category");
            }

            var categoryTags = await _tagService.GetPopularTagsAsync(DateTime.Today, category: categoryEnum, maxCount: count, cancellationToken: cancellationToken);

            return Ok(categoryTags
                .Select(t => new TagViewModel { Name = t.Name, EventCount = t.EventCount, Category = t.Category })
                .ToList());
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
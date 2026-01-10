using Events.Data.Repositories.Interfaces;
using Events.Models.Enums;
using Events.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ISubCategoryRepository subCategoryRepository, ILogger<CategoriesController> logger)
    {
        _subCategoryRepository = subCategoryRepository;
        _logger = logger;
    }

    [HttpGet("subcategories/{categoryId}")]
    [ProducesResponseType(typeof(List<SubCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SubCategoryDto>>> GetSubCategoriesByCategory(int categoryId)
    {
        try
        {
            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            var subCategoriesForCategory = allSubCategories
                .Where(sc => sc.CategoryId == categoryId)
                .Select(sc => new SubCategoryDto { Id = sc.Id, Name = sc.Name })
                .OrderBy(sc => sc.Name)
                .ToList();

            return Ok(subCategoriesForCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories for category {CategoryId}", categoryId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error getting subcategories" });
        }
    }

    [HttpGet("subcategories")]
    [ProducesResponseType(typeof(List<SubCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SubCategoryDto>>> GetSubCategoriesForCategory(
        string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return Ok(new List<SubCategoryDto>());
            }

            if (!Enum.TryParse<EventCategory>(category, true, out var parsedCategory))
            {
                return BadRequest("Invalid category name");
            }

            var subCategories = await _subCategoryRepository.GetByCategoryAsync(parsedCategory);
            
            var result = subCategories
                .Select(sc => new SubCategoryDto { Id = sc.Id, Name = sc.Name })
                .OrderBy(sc => sc.Name)
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories for category {Category}", category);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error getting subcategories" });
        }
    }
}

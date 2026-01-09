using Events.Data.Repositories.Interfaces;
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
    public async Task<ActionResult<List<object>>> GetSubCategoriesByCategory(int categoryId)
    {
        try
        {
            var allSubCategories = await _subCategoryRepository.GetAllAsync();
            var subCategoriesForCategory = allSubCategories
                .Where(sc => sc.CategoryId == categoryId)
                .Select(sc => new { id = sc.Id, name = sc.Name })
                .OrderBy(sc => sc.name)
                .ToList();

            return Ok(subCategoriesForCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories for category {CategoryId}", categoryId);
            return StatusCode(500, "Error getting subcategories");
        }
    }
}

using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Events.Web.Models;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Events.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdminRole")]
public class TagsController : Controller
{
    private readonly ILogger<TagsController> _logger;
    private readonly ITagService _tagService;
    private readonly ICategoryRepository _categoryRepository;

    public TagsController(
        ILogger<TagsController> logger,
        ITagService tagService,
        ICategoryRepository categoryRepository)
    {
        _logger = logger;
        _tagService = tagService;
        _categoryRepository = categoryRepository;
    }

    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        int? category = null,
        bool showOrphansOnly = false,
        bool showWithoutCategoryOnly = false,
        string sortBy = AdminTagSortFields.Name,
        string sortOrder = AdminTagSortOrders.Asc)
    {
        try
        {
            EventCategory? categoryFilter = null;
            if (category.HasValue && Enum.IsDefined(typeof(EventCategory), category.Value))
            {
                categoryFilter = (EventCategory)category.Value;
            }

            var query = new AdminTagQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = search,
                Category = categoryFilter,
                ShowOrphansOnly = showOrphansOnly,
                ShowWithoutCategoryOnly = showWithoutCategoryOnly,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var result = await _tagService.GetAdminTagsAsync(query);

            var tagItems = result.Tags
                .Select(t => new AdminTagListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Category = t.Category,
                    UsageCount = t.UsageCount,
                    CategoryUsage = t.CategoryUsage,
                    SubCategoryUsage = t.SubCategoryUsage,
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            var paginatedTags = new PaginatedList<AdminTagListItemViewModel>(
                tagItems,
                result.TotalCount,
                result.Page,
                result.PageSize);

            var categories = await _categoryRepository.GetAllAsync();
            var categoryOptions = BuildCategoryOptions(categories, result.Category);

            var viewModel = new AdminTagManagementViewModel
            {
                Tags = paginatedTags,
                SearchTerm = result.SearchTerm,
                CategoryFilter = result.Category,
                ShowOrphansOnly = result.ShowOrphansOnly,
                ShowWithoutCategoryOnly = result.ShowWithoutCategoryOnly,
                SortBy = result.SortBy,
                SortOrder = result.SortOrder,
                Statistics = new AdminTagStatisticsViewModel
                {
                    TotalTags = result.Statistics.TotalTags,
                    OrphanTags = result.Statistics.OrphanTags,
                    WithoutCategoryTags = result.Statistics.WithoutCategoryTags,
                    MostUsedTagName = result.Statistics.MostUsedTagName,
                    MostUsedTagCount = result.Statistics.MostUsedTagCount
                },
                CategoryOptions = categoryOptions
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin tag list");
            TempData["ErrorMessage"] = "An error occurred while loading tags.";
            return View(new AdminTagManagementViewModel());
        }
    }

    private static List<SelectListItem> BuildCategoryOptions(IEnumerable<Category> categories, EventCategory? selected)
    {
        var items = new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = string.Empty,
                Text = "Categories",
                Selected = !selected.HasValue
            }
        };

        items.AddRange(
            categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = ((int)c.CategoryType).ToString(),
                    Text = c.Name,
                    Selected = selected.HasValue && c.CategoryType == selected.Value
                }));

        return items;
    }
}

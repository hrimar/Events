using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Models.Queries;
using Events.Web.Models.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Events.Web.Services;

public class EventFilterOptionsBuilder : IEventFilterOptionsBuilder
{
    private readonly ISubCategoryRepository _subCategoryRepository;

    public EventFilterOptionsBuilder(ISubCategoryRepository subCategoryRepository)
    {
        _subCategoryRepository = subCategoryRepository;
    }

    public async Task<EventFilterOptions> BuildAdminOptionsAsync(EventListCriteria filters, IReadOnlyList<Category> categories)
    {
        var categoryItems = categories
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = filters.CategoryId == c.Id
            })
            .ToList();

        var subCategoryItems = new List<SelectListItem>();
        if (filters.CategoryId.HasValue)
        {
            var category = categories.FirstOrDefault(c => c.Id == filters.CategoryId.Value);
            if (category != null)
            {
                var subCategories = await _subCategoryRepository.GetByCategoryAsync(category.CategoryType);
                subCategoryItems = subCategories
                    .OrderBy(sc => sc.Name)
                    .Select(sc => new SelectListItem
                    {
                        Value = sc.Id.ToString(),
                        Text = sc.Name,
                        Selected = filters.SubCategoryId == sc.Id
                    })
                    .ToList();
            }
        }

        var statusItems = Enum.GetValues<EventStatus>()
            .Select(status => new SelectListItem
            {
                Value = ((int)status).ToString(),
                Text = status.ToString(),
                Selected = filters.Status == status
            })
            .Prepend(new SelectListItem { Value = "", Text = "All Statuses", Selected = !filters.Status.HasValue })
            .ToList();

        return new EventFilterOptions
        {
            Categories = categoryItems,
            SubCategories = subCategoryItems,
            Statuses = statusItems
        };
    }
}

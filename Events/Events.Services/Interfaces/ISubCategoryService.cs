using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Services.Interfaces;

public interface ISubCategoryService
{
    Task<SubCategory?> GetByIdAsync(int id);
    Task<IEnumerable<SubCategory>> GetAllAsync();
    Task<IEnumerable<SubCategory>> GetByCategoryAsync(EventCategory category);
    Task<SubCategory?> GetByEnumValueAsync(EventCategory category, int enumValue);
    Task<SubCategory?> GetByNameAsync(EventCategory category, string name);
    Task<SubCategory> CreateAsync(SubCategory subCategory);
    Task<SubCategory> UpdateAsync(SubCategory subCategory);
    Task DeleteAsync(int id);
}
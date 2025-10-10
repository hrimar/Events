using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;

namespace Events.Services.Implementations;

public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _repository;

    public SubCategoryService(ISubCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubCategory?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<IEnumerable<SubCategory>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<IEnumerable<SubCategory>> GetByCategoryAsync(EventCategory category) =>
        await _repository.GetByCategoryAsync(category);

    public async Task<SubCategory?> GetByEnumValueAsync(EventCategory category, int enumValue) =>
        await _repository.GetByEnumValueAsync(category, enumValue);

    public async Task<SubCategory?> GetByNameAsync(EventCategory category, string name) =>
        await _repository.GetByNameAsync(category, name);

    public async Task<SubCategory> CreateAsync(SubCategory subCategory) => await _repository.AddAsync(subCategory);

    public async Task<SubCategory> UpdateAsync(SubCategory subCategory) => await _repository.UpdateAsync(subCategory);

    public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
}
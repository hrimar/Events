using Events.Models.Entities;

namespace Events.Data.Repositories.Interfaces;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(int id);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByNameAsync(string name);
    Task<IEnumerable<Tag>> GetByCategoryAsync(Events.Models.Enums.EventCategory? category);
    Task<Tag> AddAsync(Tag tag);
    Task<Tag> UpdateAsync(Tag tag);
    Task DeleteAsync(int id);
}

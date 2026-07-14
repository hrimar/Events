using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class SeoMetaService : ISeoMetaService
{
    private readonly IPageSeoMetaRepository _repository;
    private readonly ILogger<SeoMetaService> _logger;

    public SeoMetaService(IPageSeoMetaRepository repository, ILogger<SeoMetaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<PageSeoMeta>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<PageSeoMeta?> GetByKeyAsync(string pageKey) => await _repository.GetByKeyAsync(pageKey);

    public async Task SaveAllAsync(IEnumerable<PageSeoMeta> pages)
    {
        await _repository.UpdateManyAsync(pages);
        _logger.LogInformation("Page SEO meta updated");
    }
}

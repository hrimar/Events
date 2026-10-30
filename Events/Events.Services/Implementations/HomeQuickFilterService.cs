using System.Text.Json;
using Events.Data.Repositories.Interfaces;
using Events.Services.Caching;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Interfaces;
using Events.Services.Models.Admin;
using Microsoft.Extensions.Logging;

namespace Events.Services.Implementations;

public class HomeQuickFilterService : IHomeQuickFilterService
{
    // Mirrors the column sizes in EventsDbContext (Values is stored as a JSON array).
    private const int MaxLabelLength = 50;
    private const int MaxValuesJsonLength = 500;

    private readonly IHomeQuickFilterRepository _repository;
    private readonly IEventCacheInvalidator _cacheInvalidator;
    private readonly ILogger<HomeQuickFilterService> _logger;

    public HomeQuickFilterService(
        IHomeQuickFilterRepository repository,
        IEventCacheInvalidator cacheInvalidator,
        ILogger<HomeQuickFilterService> logger)
    {
        _repository = repository;
        _cacheInvalidator = cacheInvalidator;
        _logger = logger;
    }

    public async Task<List<HomeQuickFilter>> GetAllAsync() => await _repository.GetAllAsync();

    public async Task<List<HomeQuickFilter>> GetActiveAsync() =>
        (await _repository.GetActiveAsync()).Take(HomeQuickFilter.MaxActive).ToList();

    public async Task<HomeQuickFilter?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<HomeQuickFilterResult> CreateAsync(HomeQuickFilterInput input)
    {
        var all = await _repository.GetAllAsync();

        var error = Validate(input, out var values);
        if (error == null && input.IsActive && CountActive(all, excludeId: null) >= HomeQuickFilter.MaxActive)
            error = ActiveLimitError();

        if (error != null)
            return HomeQuickFilterResult.Failure(error);

        var filter = new HomeQuickFilter
        {
            LabelBg = input.LabelBg.Trim(),
            LabelEn = input.LabelEn.Trim(),
            Type = input.Type,
            Values = values,
            SortOrder = all.Count == 0 ? 1 : all.Max(f => f.SortOrder) + 1,
            IsActive = input.IsActive,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(filter);
        _cacheInvalidator.Invalidate();
        _logger.LogInformation("Homepage quick filter {QuickFilterId} '{Label}' created", filter.Id, filter.LabelEn);
        return HomeQuickFilterResult.Success();
    }

    public async Task<HomeQuickFilterResult> UpdateAsync(int id, HomeQuickFilterInput input)
    {
        var all = await _repository.GetAllAsync();
        var filter = all.FirstOrDefault(f => f.Id == id);
        if (filter == null)
            return NotFound();

        var error = Validate(input, out var values);
        if (error == null && input.IsActive && !filter.IsActive && CountActive(all, excludeId: id) >= HomeQuickFilter.MaxActive)
            error = ActiveLimitError();

        if (error != null)
            return HomeQuickFilterResult.Failure(error);

        filter.LabelBg = input.LabelBg.Trim();
        filter.LabelEn = input.LabelEn.Trim();
        filter.Type = input.Type;
        filter.Values = values;
        filter.IsActive = input.IsActive;
        filter.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        _cacheInvalidator.Invalidate();
        _logger.LogInformation("Homepage quick filter {QuickFilterId} '{Label}' updated", filter.Id, filter.LabelEn);
        return HomeQuickFilterResult.Success();
    }

    public async Task<HomeQuickFilterResult> DeleteAsync(int id)
    {
        var filter = await _repository.GetByIdAsync(id);
        if (filter == null)
            return NotFound();

        await _repository.DeleteAsync(filter);
        _cacheInvalidator.Invalidate();
        _logger.LogInformation("Homepage quick filter {QuickFilterId} '{Label}' deleted", filter.Id, filter.LabelEn);
        return HomeQuickFilterResult.Success();
    }

    public async Task<HomeQuickFilterResult> SetActiveAsync(int id, bool isActive)
    {
        var all = await _repository.GetAllAsync();
        var filter = all.FirstOrDefault(f => f.Id == id);
        if (filter == null)
            return NotFound();

        if (filter.IsActive == isActive)
            return HomeQuickFilterResult.Success();

        if (isActive && CountActive(all, excludeId: id) >= HomeQuickFilter.MaxActive)
            return HomeQuickFilterResult.Failure(ActiveLimitError());

        filter.IsActive = isActive;
        filter.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        _cacheInvalidator.Invalidate();
        _logger.LogInformation("Homepage quick filter {QuickFilterId} active = {IsActive}", filter.Id, isActive);
        return HomeQuickFilterResult.Success();
    }

    public async Task<HomeQuickFilterResult> MoveAsync(int id, int offset)
    {
        if (offset is not (-1 or 1))
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be -1 or 1.");

        var all = await _repository.GetAllAsync();
        var index = all.FindIndex(f => f.Id == id);
        if (index < 0)
            return NotFound();

        var targetIndex = index + offset;

        // Already first or last: nothing to do.
        if (targetIndex < 0 || targetIndex >= all.Count)
            return HomeQuickFilterResult.Success();

        (all[index], all[targetIndex]) = (all[targetIndex], all[index]);

        // Renumbered 1..n on every move, so duplicate or sparse SortOrder values never accumulate.
        for (var i = 0; i < all.Count; i++)
        {
            all[i].SortOrder = i + 1;
        }

        await _repository.SaveChangesAsync();
        _cacheInvalidator.Invalidate();
        return HomeQuickFilterResult.Success();
    }

    // Returns an error message, or null with the normalized values (canonical category names,
    // trimmed distinct tag names, nothing for Free).
    private static string? Validate(HomeQuickFilterInput input, out List<string> values)
    {
        values = new List<string>();

        if (string.IsNullOrWhiteSpace(input.LabelBg) || string.IsNullOrWhiteSpace(input.LabelEn))
            return "Both labels (BG and EN) are required.";

        if (input.LabelBg.Trim().Length > MaxLabelLength || input.LabelEn.Trim().Length > MaxLabelLength)
            return $"Labels can be at most {MaxLabelLength} characters long.";

        if (!Enum.IsDefined(input.Type))
            return "Unknown filter type.";

        var rawValues = input.Values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        switch (input.Type)
        {
            case HomeQuickFilterType.Free:
                return null;

            case HomeQuickFilterType.Categories:
                foreach (var rawValue in rawValues)
                {
                    var category = TryParseCategory(rawValue);
                    if (category == null)
                        return $"Unknown category: {rawValue}.";

                    values.Add(category.Value.ToString());
                }

                values = values.Distinct().ToList();
                break;

            case HomeQuickFilterType.Tags:
                values = rawValues;
                break;
        }

        if (values.Count == 0)
            return "Select at least one value.";

        if (JsonSerializer.Serialize(values).Length > MaxValuesJsonLength)
            return "Too many values for one filter.";

        return null;
    }

    // Only named, real categories (not "Undefined", numbers or comma-combined names).
    private static EventCategory? TryParseCategory(string value) =>
        !value.Contains(',')
        && !int.TryParse(value, out _)
        && Enum.TryParse<EventCategory>(value, ignoreCase: true, out var category)
        && Enum.IsDefined(category)
        && category != EventCategory.Undefined
            ? category
            : null;

    private static int CountActive(IEnumerable<HomeQuickFilter> filters, int? excludeId) =>
        filters.Count(f => f.IsActive && f.Id != excludeId);

    private static string ActiveLimitError() =>
        $"At most {HomeQuickFilter.MaxActive} quick filters can be active. Deactivate another one first.";

    private static HomeQuickFilterResult NotFound() => HomeQuickFilterResult.Failure("Quick filter not found.");
}

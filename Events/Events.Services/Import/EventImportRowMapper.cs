using System.Globalization;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Helpers;
using Events.Services.Import.Models;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Import;

public class EventImportRowMapper : IEventImportRowMapper
{
    private const int NameMaxLength = 200;
    private const int CityMaxLength = 100;
    private const int LocationMaxLength = 300;
    private const int DescriptionMaxLength = 4000;
    private const int UrlMaxLength = 500;

    private static readonly string[] TagDelimiters = { "," };

    private static readonly Dictionary<string, EventStatus> StatusSynonyms = new(StringComparer.OrdinalIgnoreCase)
    {
        ["draft"] = EventStatus.Draft,
        ["черновa"] = EventStatus.Draft,
        ["чернова"] = EventStatus.Draft,
        ["published"] = EventStatus.Published,
        ["active"] = EventStatus.Published,
        ["публикувано"] = EventStatus.Published,
        ["активно"] = EventStatus.Published,
        ["cancelled"] = EventStatus.Cancelled,
        ["canceled"] = EventStatus.Cancelled,
        ["отменено"] = EventStatus.Cancelled,
        ["отменен"] = EventStatus.Cancelled,
        ["postponed"] = EventStatus.Postponed,
        ["отложено"] = EventStatus.Postponed,
        ["отложен"] = EventStatus.Postponed,
        ["soldout"] = EventStatus.SoldOut,
        ["sold out"] = EventStatus.SoldOut,
        ["разпродадено"] = EventStatus.SoldOut,
        ["билетите свършиха"] = EventStatus.SoldOut,
    };

    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryService _subCategoryService;
    private readonly ITagService _tagService;
    private readonly IVenueService _venueService;
    private readonly ILogger<EventImportRowMapper> _logger;

    private List<Category>? _categoriesCache;

    public EventImportRowMapper(
        ICategoryRepository categoryRepository,
        ISubCategoryService subCategoryService,
        ITagService tagService,
        IVenueService venueService,
        ILogger<EventImportRowMapper> logger)
    {
        _categoryRepository = categoryRepository;
        _subCategoryService = subCategoryService;
        _tagService = tagService;
        _venueService = venueService;
        _logger = logger;
    }

    public async Task<ImportRowResult> MapRowAsync(RawImportRow row, EventImportColumnMap columnMap, CancellationToken cancellationToken = default)
    {
        var result = new ImportRowResult { RowNumber = row.RowNumber };

        MapName(row, columnMap, result);
        await MapDateAndTimeAsync(row, columnMap, result);
        MapCity(row, columnMap, result);
        await MapLocationAsync(row, columnMap, result);
        MapDescription(row, columnMap, result);
        MapImageUrl(row, columnMap, result);
        MapTicketUrl(row, columnMap, result);
        await MapCategoryAsync(row, columnMap, result);
        await MapSubCategoryAsync(row, columnMap, result);
        await MapTagsAsync(row, columnMap, result);
        MapFreeEvent(row, columnMap, result);
        MapStatus(row, columnMap, result);
        MapFeatured(row, columnMap, result);

        return result;
    }

    private static string? GetValue(RawImportRow row, string columnName) =>
        row.Values.TryGetValue(columnName, out var value) ? value : null;

    private static void MapName(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.EventName)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.AddError("Event Name is required.");
            return;
        }

        result.Name = StringTruncationHelper.TruncateString(raw, NameMaxLength);
    }

    private Task MapDateAndTimeAsync(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var rawDate = GetValue(row, columnMap.Date)?.Trim();
        if (string.IsNullOrWhiteSpace(rawDate))
        {
            result.AddError("Date is required.");
        }
        else if (TryParseDate(rawDate, out var date))
        {
            result.Date = date;
        }
        else
        {
            result.AddError($"Date '{rawDate}' could not be parsed.");
        }

        var rawStartTime = GetValue(row, columnMap.StartTime)?.Trim();
        if (!string.IsNullOrWhiteSpace(rawStartTime))
        {
            if (TimeSpan.TryParse(rawStartTime, CultureInfo.InvariantCulture, out var startTime))
            {
                result.StartTime = startTime;
            }
            else
            {
                result.AddWarning($"Start Time '{rawStartTime}' could not be parsed and was ignored.");
            }
        }

        return Task.CompletedTask;
    }

    private static bool TryParseDate(string rawDate, out DateTime date)
    {
        string[] formats =
        {
            "dd.MM.yyyy", "d.M.yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "dd-MM-yyyy"
        };

        if (DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return true;
        }

        if (DateTime.TryParse(rawDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return true;
        }

        return DateTime.TryParse(rawDate, CultureInfo.GetCultureInfo("bg-BG"), DateTimeStyles.None, out date);
    }

    private static void MapCity(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.City)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.AddError("City is required.");
            return;
        }

        result.City = StringTruncationHelper.TruncateString(raw, CityMaxLength);
    }

    private async Task MapLocationAsync(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.VenueLocation)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.AddError("Venue/Location is required.");
            return;
        }

        result.Location = StringTruncationHelper.TruncateString(raw, LocationMaxLength);

        // Always keep the raw Location text regardless of whether a canonical venue is matched.
        result.CanonicalVenueId = await _venueService.FindCanonicalVenueIdAsync(result.Location);
    }

    private static void MapDescription(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.Description)?.Trim();
        result.Description = string.IsNullOrWhiteSpace(raw) ? null : StringTruncationHelper.TruncateString(raw, DescriptionMaxLength);
    }

    private static void MapImageUrl(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.ImageUrl)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        result.ImageUrl = StringTruncationHelper.TruncateString(raw, UrlMaxLength);
        if (!Uri.TryCreate(raw, UriKind.Absolute, out _))
        {
            result.AddWarning($"Image URL '{raw}' does not look like a valid absolute URL.");
        }
    }

    private static void MapTicketUrl(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.TicketUrl)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        result.TicketUrl = StringTruncationHelper.TruncateString(raw, UrlMaxLength);
        if (!Uri.TryCreate(raw, UriKind.Absolute, out _))
        {
            result.AddWarning($"Ticket URL '{raw}' does not look like a valid absolute URL.");
        }
    }

    private async Task MapCategoryAsync(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.Category)?.Trim();
        result.RawCategoryText = raw ?? "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            result.AddError("Category is required.");
            return;
        }

        var categories = await GetCategoriesAsync();
        var normalized = Normalize(raw);

        var match = categories.FirstOrDefault(c => Normalize(c.Name) == normalized)
                    ?? categories.FirstOrDefault(c => Normalize(c.Name).StartsWith(normalized, StringComparison.OrdinalIgnoreCase)
                                                       || normalized.StartsWith(Normalize(c.Name), StringComparison.OrdinalIgnoreCase));

        if (match == null)
        {
            result.AddWarning($"Category '{raw}' was not recognized. Please pick one manually.");
            return;
        }

        result.CategoryId = match.Id;
    }

    private async Task MapSubCategoryAsync(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.SubCategory)?.Trim();
        result.RawSubCategoryText = raw ?? "";

        if (string.IsNullOrWhiteSpace(raw) || result.CategoryId == null)
        {
            // Without a resolved Category there's nothing to validate the subcategory against;
            // the Category warning/error above already flags the row.
            return;
        }

        var categories = await GetCategoriesAsync();
        var category = categories.First(c => c.Id == result.CategoryId);

        var enumValue = SubCategoryMapper.MapSubCategoryToEnumValue(category.CategoryType, raw, _logger, allowOtherFallback: false);
        if (enumValue == null)
        {
            result.AddWarning($"Subcategory '{raw}' was not recognized for category '{category.Name}'. Please pick one manually.");
            return;
        }

        var subCategory = await _subCategoryService.GetByEnumValueAsync(category.CategoryType, enumValue.Value);
        if (subCategory == null)
        {
            result.AddWarning($"Subcategory '{raw}' was not recognized for category '{category.Name}'. Please pick one manually.");
            return;
        }

        result.SubCategoryId = subCategory.Id;
    }

    private async Task MapTagsAsync(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.Tags)?.Trim();
        result.RawTagsText = raw ?? "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        var candidates = raw.Split(TagDelimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (candidates.Count > 3)
        {
            result.AddWarning($"More than 3 tags were provided ('{raw}'); only the first 3 will be considered.");
            candidates = candidates.Take(3).ToList();
        }

        foreach (var candidate in candidates)
        {
            var normalized = TagNameNormalizer.Normalize(candidate);
            if (normalized == null)
            {
                result.UnmatchedTagNames.Add(candidate);
                result.AddWarning($"Tag '{candidate}' is not a valid tag name.");
                continue;
            }

            var tag = await _tagService.GetTagByNameAsync(normalized);
            if (tag == null)
            {
                result.UnmatchedTagNames.Add(candidate);
                result.AddWarning($"Tag '{candidate}' was not recognized. Please pick an existing tag manually.");
                continue;
            }

            result.MatchedTagIds.Add(tag.Id);
        }
    }

    private static void MapFreeEvent(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.FreeEventText)?.Trim();
        result.RawFreeEventText = raw ?? "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            result.IsFree = false;
            result.Price = null;
            return;
        }

        // The column holds free-form notes (e.g. a recurrence note), not a clean boolean, so any
        // non-blank text is treated as "free" but the row is always flagged for the admin to confirm.
        result.IsFree = true;
        result.Price = null;
        result.AddWarning($"Free-event note present ('{raw}') — please verify IsFree/Price.");
    }

    private static void MapStatus(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.EventStatus)?.Trim();
        result.RawStatusText = raw ?? "";

        if (string.IsNullOrWhiteSpace(raw))
        {
            result.Status = EventStatus.Draft;
            return;
        }

        if (Enum.TryParse<EventStatus>(raw, ignoreCase: true, out var status))
        {
            result.Status = status;
            return;
        }

        if (StatusSynonyms.TryGetValue(raw, out var synonymStatus))
        {
            result.Status = synonymStatus;
            return;
        }

        result.AddWarning($"Event Status '{raw}' was not recognized. Please pick one manually.");
    }

    private static void MapFeatured(RawImportRow row, EventImportColumnMap columnMap, ImportRowResult result)
    {
        var raw = GetValue(row, columnMap.FeaturedEvent)?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            result.IsFeatured = false;
            return;
        }

        result.IsFeatured = raw.ToLowerInvariant() switch
        {
            "yes" or "y" or "да" or "1" or "true" => true,
            _ => false
        };
    }

    private async Task<List<Category>> GetCategoriesAsync()
    {
        _categoriesCache ??= (await _categoryRepository.GetAllAsync()).ToList();
        return _categoriesCache;
    }

    private static string Normalize(string value) => value.Trim().ToLowerInvariant();
}

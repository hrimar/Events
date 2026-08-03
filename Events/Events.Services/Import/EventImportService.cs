using Events.Models.Entities;
using Events.Services.Import.Models;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Events.Services.Import;

public class EventImportService : IEventImportService
{
    private readonly IEventImportFileParserFactory _parserFactory;
    private readonly IEventImportRowMapper _rowMapper;
    private readonly IEventImportDuplicateDetector _duplicateDetector;
    private readonly IEventService _eventService;
    private readonly ITagService _tagService;
    private readonly ILogger<EventImportService> _logger;

    public EventImportService(
        IEventImportFileParserFactory parserFactory,
        IEventImportRowMapper rowMapper,
        IEventImportDuplicateDetector duplicateDetector,
        IEventService eventService,
        ITagService tagService,
        ILogger<EventImportService> logger)
    {
        _parserFactory = parserFactory;
        _rowMapper = rowMapper;
        _duplicateDetector = duplicateDetector;
        _eventService = eventService;
        _tagService = tagService;
        _logger = logger;
    }

    public async Task<EventImportBatch> ParseAndValidateAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var parser = _parserFactory.GetParser(fileName);
        var sheet = await parser.ParseAsync(fileStream, fileName, cancellationToken);

        var batch = new EventImportBatch { OriginalFileName = fileName };

        foreach (var row in sheet.Rows)
        {
            var mapped = await _rowMapper.MapRowAsync(row, EventImportColumnMap.Default, cancellationToken);

            var (isDuplicate, existingEventId) = await _duplicateDetector.FindExistingDuplicateAsync(mapped, cancellationToken);
            if (isDuplicate)
            {
                mapped.IsDuplicate = true;
                mapped.DuplicateEventId = existingEventId;
                mapped.AddWarning($"Possible duplicate of existing event #{existingEventId}.");
            }

            batch.Rows.Add(mapped);
        }

        _duplicateDetector.DetectIntraBatchDuplicates(batch.Rows);

        return batch;
    }

    public async Task<EventImportCommitResult> CommitAsync(EventImportBatch batch, CancellationToken cancellationToken = default)
    {
        var result = new EventImportCommitResult();

        foreach (var row in batch.Rows)
        {
            if (row.Excluded)
            {
                result.SkippedExcludedCount++;
                continue;
            }

            if (row.Severity == ImportRowSeverity.Error || row.CategoryId == null || row.Status == null
                || row.Date == null || string.IsNullOrWhiteSpace(row.Name)
                || string.IsNullOrWhiteSpace(row.City) || string.IsNullOrWhiteSpace(row.Location))
            {
                result.FailedCount++;
                result.Failures.Add((row.RowNumber, "Row is missing required data and was not imported."));
                continue;
            }

            try
            {
                var eventEntity = new Event
                {
                    Name = row.Name,
                    Date = row.Date.Value,
                    StartTime = row.StartTime,
                    City = row.City,
                    Location = row.Location,
                    Description = row.Description,
                    ImageUrl = row.ImageUrl,
                    TicketUrl = row.TicketUrl,
                    IsFree = row.IsFree,
                    Price = row.Price,
                    IsFeatured = row.IsFeatured,
                    CategoryId = row.CategoryId.Value,
                    SubCategoryId = row.SubCategoryId,
                    CanonicalVenueId = row.CanonicalVenueId,
                    Status = row.Status.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdEvent = await _eventService.CreateEventAsync(eventEntity);

                if (row.MatchedTagIds.Count > 0)
                {
                    await _tagService.BulkAddTagsToEventAsync(createdEvent.Id, row.MatchedTagIds);
                }

                result.CreatedCount++;
                result.CreatedEventIds.Add(createdEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import row {RowNumber} ('{Name}') from batch {BatchId}", row.RowNumber, row.Name, batch.BatchId);
                result.FailedCount++;
                result.Failures.Add((row.RowNumber, ex.Message));
            }
        }

        return result;
    }
}

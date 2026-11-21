using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
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
    private const int JobItemDetailMaxLength = 500;
    private const int JobItemEventNameMaxLength = 200;

    private readonly ITagService _tagService;
    private readonly IEventImportJobRepository _jobRepository;
    private readonly ILogger<EventImportService> _logger;

    public EventImportService(
        IEventImportFileParserFactory parserFactory,
        IEventImportRowMapper rowMapper,
        IEventImportDuplicateDetector duplicateDetector,
        IEventService eventService,
        ITagService tagService,
        IEventImportJobRepository jobRepository,
        ILogger<EventImportService> logger)
    {
        _parserFactory = parserFactory;
        _rowMapper = rowMapper;
        _duplicateDetector = duplicateDetector;
        _eventService = eventService;
        _tagService = tagService;
        _jobRepository = jobRepository;
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

    public async Task<EventImportCommitResult> CommitAsync(EventImportBatch batch, string createdBy, CancellationToken cancellationToken = default)
    {
        var result = new EventImportCommitResult();
        var jobItems = new List<EventImportJobItem>();

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
                const string missingDataError = "Row is missing required data and was not imported.";
                result.FailedCount++;
                result.Failures.Add((row.RowNumber, missingDataError));
                jobItems.Add(CreateNotImportedItem(row, missingDataError));
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
                    // No external address as the main image (GS-REQ-009, 9.3.2): the background job stores a local
                    // copy and sets ImageUrl/ThumbnailUrl; until then (or if it fails) the default image is shown.
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
                jobItems.Add(CreateImportedItem(row, createdEvent.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import row {RowNumber} ('{Name}') from batch {BatchId}", row.RowNumber, row.Name, batch.BatchId);
                result.FailedCount++;
                result.Failures.Add((row.RowNumber, ex.Message));
                jobItems.Add(CreateNotImportedItem(row, ex.Message));
            }
        }

        result.JobId = await SaveImageJobAsync(batch, createdBy, result, jobItems, cancellationToken);

        return result;
    }

    // Saves the image job and its report lines (GS-REQ-009, 9.3.3, 9.3.4). The events are already created, so a
    // failure here is logged and the import still succeeds (the images are then not copied for this batch).
    private async Task<int?> SaveImageJobAsync(
        EventImportBatch batch, string createdBy, EventImportCommitResult result, List<EventImportJobItem> items, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var hasPendingItems = items.Any(i => i.Outcome == EventImportImageOutcome.Pending);

        var job = new EventImportJob
        {
            BatchId = batch.BatchId,
            FileName = Truncate(batch.OriginalFileName, 260),
            CreatedBy = Truncate(createdBy, 256),
            CreatedAtUtc = now,
            CompletedAtUtc = hasPendingItems ? null : now,
            Status = hasPendingItems ? EventImportJobStatus.Queued : EventImportJobStatus.Completed,
            TotalRows = batch.Rows.Count,
            CreatedCount = result.CreatedCount,
            SkippedExcludedCount = result.SkippedExcludedCount,
            FailedCount = result.FailedCount,
            Items = items
        };

        try
        {
            await _jobRepository.AddAsync(job, cancellationToken);
            return job.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save the image job for import batch {BatchId}", batch.BatchId);
            return null;
        }
    }

    // Rows with "Image URL" or "Ticket URL" wait for the background job; rows with neither have nothing to fetch.
    private static EventImportJobItem CreateImportedItem(ImportRowResult row, int eventId)
    {
        var hasSource = !string.IsNullOrWhiteSpace(row.ImageUrl) || !string.IsNullOrWhiteSpace(row.TicketUrl);
        return new EventImportJobItem
        {
            RowNumber = row.RowNumber,
            EventId = eventId,
            EventName = Truncate(row.Name ?? string.Empty, JobItemEventNameMaxLength),
            SourceImageUrl = row.ImageUrl,
            PageUrl = row.TicketUrl,
            Outcome = hasSource ? EventImportImageOutcome.Pending : EventImportImageOutcome.NoImage
        };
    }

    private static EventImportJobItem CreateNotImportedItem(ImportRowResult row, string error) => new()
    {
        RowNumber = row.RowNumber,
        EventName = Truncate(row.Name ?? string.Empty, JobItemEventNameMaxLength),
        SourceImageUrl = row.ImageUrl,
        PageUrl = row.TicketUrl,
        Outcome = EventImportImageOutcome.RowNotImported,
        Detail = Truncate(error, JobItemDetailMaxLength)
    };

    private static string Truncate(string value, int maxLength) => value.Length <= maxLength ? value : value[..maxLength];
}

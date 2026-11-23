using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Implementations;
using Events.Services.Import;
using Events.Services.Import.Models;
using Events.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Import;

public class EventImportServiceTests
{
    private readonly Mock<IEventImportFileParserFactory> _parserFactoryMock = new();
    private readonly Mock<IEventImportFileParser> _parserMock = new();
    private readonly Mock<IEventImportRowMapper> _rowMapperMock = new();
    private readonly Mock<IEventImportDuplicateDetector> _duplicateDetectorMock = new();
    private readonly Mock<IEventService> _eventServiceMock = new();
    private readonly Mock<ITagService> _tagServiceMock = new();
    private readonly Mock<IEventImportJobRepository> _jobRepositoryMock = new();
    private readonly Mock<ILogger<EventImportService>> _loggerMock = new();
    private static readonly DateTimeOffset ImportTime = new(2026, 11, 23, 14, 30, 0, TimeSpan.Zero);

    public EventImportServiceTests()
    {
        _parserFactoryMock.Setup(f => f.GetParser(It.IsAny<string>())).Returns(_parserMock.Object);
    }

    private EventImportService CreateEventImportService() => new(
        _parserFactoryMock.Object,
        _rowMapperMock.Object,
        _duplicateDetectorMock.Object,
        _eventServiceMock.Object,
        _tagServiceMock.Object,
        _jobRepositoryMock.Object,
        new FixedTimeProvider(ImportTime),
        _loggerMock.Object);

    private static ImportRowResult CreateValidRow(int rowNumber = 1, string name = "Concert") => new()
    {
        RowNumber = rowNumber,
        Name = name,
        Date = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        City = "Sofia",
        Location = "NDK",
        CategoryId = 1,
        Status = EventStatus.Published
    };

    // ParseAndValidateAsync

    [Fact]
    public async Task ParseAndValidateAsync_UnsupportedFileExtension_PropagatesFactoryException()
    {
        _parserFactoryMock.Setup(f => f.GetParser("file.txt")).Throws(new NotSupportedException("Unsupported file type"));

        await Assert.ThrowsAsync<NotSupportedException>(
            () => CreateEventImportService().ParseAndValidateAsync(Stream.Null, "file.txt"));
    }

    [Fact]
    public async Task ParseAndValidateAsync_MapsEveryRowAndFlagsDuplicatesAgainstExistingEvents()
    {
        // Arrange - one row maps to a row that the detector reports as a duplicate of event #42
        var sheet = new RawImportSheet { Rows = { new RawImportRow { RowNumber = 1 } } };
        _parserMock.Setup(p => p.ParseAsync(Stream.Null, "file.csv", It.IsAny<CancellationToken>())).ReturnsAsync(sheet);

        var mappedRow = CreateValidRow();
        _rowMapperMock
            .Setup(m => m.MapRowAsync(sheet.Rows[0], EventImportColumnMap.Default, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mappedRow);

        _duplicateDetectorMock
            .Setup(d => d.FindExistingDuplicateAsync(mappedRow, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, 42));

        // Act
        var batch = await CreateEventImportService().ParseAndValidateAsync(Stream.Null, "file.csv");

        // Assert
        var row = Assert.Single(batch.Rows);
        Assert.True(row.IsDuplicate);
        Assert.Equal(42, row.DuplicateEventId);
        Assert.Contains(row.Messages, msg => msg.Contains("#42"));
        _duplicateDetectorMock.Verify(d => d.DetectIntraBatchDuplicates(batch.Rows), Times.Once);
    }

    [Fact]
    public async Task ParseAndValidateAsync_NoDuplicateFound_RowIsNotFlagged()
    {
        var sheet = new RawImportSheet { Rows = { new RawImportRow { RowNumber = 1 } } };
        _parserMock.Setup(p => p.ParseAsync(Stream.Null, "file.csv", It.IsAny<CancellationToken>())).ReturnsAsync(sheet);

        var mappedRow = CreateValidRow();
        _rowMapperMock
            .Setup(m => m.MapRowAsync(It.IsAny<RawImportRow>(), It.IsAny<EventImportColumnMap>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mappedRow);
        _duplicateDetectorMock
            .Setup(d => d.FindExistingDuplicateAsync(It.IsAny<ImportRowResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (int?)null));

        var batch = await CreateEventImportService().ParseAndValidateAsync(Stream.Null, "file.csv");

        Assert.False(Assert.Single(batch.Rows).IsDuplicate);
    }

    // CommitAsync - validation

    [Fact]
    public async Task CommitAsync_ExcludedRow_IsSkippedAndNotCreated()
    {
        var batch = new EventImportBatch { Rows = { new ImportRowResult { RowNumber = 1, Excluded = true } } };

        var result = await CreateEventImportService().CommitAsync(batch, "admin");

        Assert.Equal(1, result.SkippedExcludedCount);
        Assert.Equal(0, result.CreatedCount);
        _eventServiceMock.Verify(s => s.CreateEventAsync(It.IsAny<Event>()), Times.Never);
    }

    [Theory]
    [InlineData(nameof(ImportRowResult.Severity))]
    [InlineData(nameof(ImportRowResult.CategoryId))]
    [InlineData(nameof(ImportRowResult.Status))]
    [InlineData(nameof(ImportRowResult.Date))]
    [InlineData(nameof(ImportRowResult.Name))]
    [InlineData(nameof(ImportRowResult.City))]
    [InlineData(nameof(ImportRowResult.Location))]
    public async Task CommitAsync_RowMissingRequiredField_CountsAsFailedWithoutCreatingEvent(string missingField)
    {
        // Arrange - start from an otherwise-valid row and blank out exactly one required field
        var row = CreateValidRow();
        switch (missingField)
        {
            case nameof(ImportRowResult.Severity): row.Severity = ImportRowSeverity.Error; break;
            case nameof(ImportRowResult.CategoryId): row.CategoryId = null; break;
            case nameof(ImportRowResult.Status): row.Status = null; break;
            case nameof(ImportRowResult.Date): row.Date = null; break;
            case nameof(ImportRowResult.Name): row.Name = ""; break;
            case nameof(ImportRowResult.City): row.City = ""; break;
            case nameof(ImportRowResult.Location): row.Location = ""; break;
        }
        var batch = new EventImportBatch { Rows = { row } };

        // Act
        var result = await CreateEventImportService().CommitAsync(batch, "admin");

        // Assert
        Assert.Equal(1, result.FailedCount);
        Assert.Equal(0, result.CreatedCount);
        _eventServiceMock.Verify(s => s.CreateEventAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task CommitAsync_ValidRowWithMatchedTags_CreatesEventAndAssignsTags()
    {
        // Arrange
        var row = CreateValidRow();
        row.MatchedTagIds.AddRange([10, 11]);
        var batch = new EventImportBatch { Rows = { row } };

        _eventServiceMock
            .Setup(s => s.CreateEventAsync(It.Is<Event>(e => e.Name == "Concert")))
            .ReturnsAsync(new Event { Id = 99, Name = "Concert" });

        // Act
        var result = await CreateEventImportService().CommitAsync(batch, "admin");

        // Assert
        Assert.Equal(1, result.CreatedCount);
        Assert.Equal([99], result.CreatedEventIds);
        _tagServiceMock.Verify(t => t.BulkAddTagsToEventAsync(99, row.MatchedTagIds), Times.Once);
    }

    [Fact]
    public async Task CommitAsync_ValidRowWithoutTags_CreatesEventWithoutCallingTagService()
    {
        var batch = new EventImportBatch { Rows = { CreateValidRow() } };
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event { Id = 1 });

        await CreateEventImportService().CommitAsync(batch, "admin");

        _tagServiceMock.Verify(t => t.BulkAddTagsToEventAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task CommitAsync_OneRowThrows_OtherRowsStillCommitAndFailureIsRecorded()
    {
        // Arrange - three valid rows; the middle one fails when EventService.CreateEventAsync is called
        var goodRow1 = CreateValidRow(1, "First");
        var badRow = CreateValidRow(2, "Second");
        var goodRow2 = CreateValidRow(3, "Third");
        var batch = new EventImportBatch { Rows = { goodRow1, badRow, goodRow2 } };

        _eventServiceMock.Setup(s => s.CreateEventAsync(It.Is<Event>(e => e.Name == "First")))
            .ReturnsAsync(new Event { Id = 1, Name = "First" });
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.Is<Event>(e => e.Name == "Second")))
            .ThrowsAsync(new InvalidOperationException("db error"));
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.Is<Event>(e => e.Name == "Third")))
            .ReturnsAsync(new Event { Id = 3, Name = "Third" });

        // Act
        var result = await CreateEventImportService().CommitAsync(batch, "admin");

        // Assert - the failure on row 2 doesn't stop rows 1 and 3 from being committed
        Assert.Equal(2, result.CreatedCount);
        Assert.Equal([1, 3], result.CreatedEventIds);
        Assert.Equal(1, result.FailedCount);
        Assert.Contains(result.Failures, f => f.RowNumber == 2 && f.Error == "db error");
    }

    // CommitAsync - image job (GS-REQ-009, 9.3.2-9.3.4)

    [Fact]
    public async Task CommitAsync_RowWithImageUrl_EventGetsNoExternalImageAndItemIsPending()
    {
        var row = CreateValidRow();
        row.ImageUrl = "https://scontent.fbcdn.net/v/photo.jpg?oe=6A1B2C3D";
        Event? created = null;
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => created = e)
            .ReturnsAsync(new Event { Id = 7 });
        var job = CaptureSavedJob();

        var result = await CreateEventImportService().CommitAsync(new EventImportBatch { Rows = { row } }, "admin");

        Assert.Null(created!.ImageUrl);
        Assert.Null(created.ThumbnailUrl);
        var item = Assert.Single(job().Items);
        Assert.Equal(EventImportImageOutcome.Pending, item.Outcome);
        Assert.Equal(7, item.EventId);
        Assert.Equal(row.ImageUrl, item.SourceImageUrl);
        Assert.Equal(EventImportJobStatus.Queued, job().Status);
        Assert.Equal(42, result.JobId);
    }

    [Fact]
    public async Task CommitAsync_JobHoldsCountsAndOneItemPerNonExcludedRow()
    {
        var withPage = CreateValidRow(1, "With page");
        withPage.TicketUrl = "https://www.facebook.com/events/123/";
        var withoutSources = CreateValidRow(2, "No sources");
        var excluded = new ImportRowResult { RowNumber = 3, Excluded = true };
        var missingData = CreateValidRow(4, "Missing city");
        missingData.City = "";
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event { Id = 1 });
        var job = CaptureSavedJob();
        var batch = new EventImportBatch { OriginalFileName = "facebook.xlsx", Rows = { withPage, withoutSources, excluded, missingData } };

        await CreateEventImportService().CommitAsync(batch, "owner@go-sofia");

        var saved = job();
        Assert.Equal("facebook.xlsx", saved.FileName);
        Assert.Equal("owner@go-sofia", saved.CreatedBy);
        Assert.Equal(batch.BatchId, saved.BatchId);
        Assert.Equal((4, 2, 1, 1), (saved.TotalRows, saved.CreatedCount, saved.SkippedExcludedCount, saved.FailedCount));
        Assert.Equal([1, 2, 4], saved.Items.Select(i => i.RowNumber));
        Assert.Equal(EventImportImageOutcome.Pending, saved.Items.Single(i => i.RowNumber == 1).Outcome);
        Assert.Equal(EventImportImageOutcome.NoImage, saved.Items.Single(i => i.RowNumber == 2).Outcome);
        var notImported = saved.Items.Single(i => i.RowNumber == 4);
        Assert.Equal(EventImportImageOutcome.RowNotImported, notImported.Outcome);
        Assert.Null(notImported.EventId);
        Assert.NotNull(notImported.Detail);
    }

    [Fact]
    public async Task CommitAsync_NothingToFetch_JobIsCompletedImmediately()
    {
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event { Id = 1 });
        var job = CaptureSavedJob();

        await CreateEventImportService().CommitAsync(new EventImportBatch { Rows = { CreateValidRow() } }, "admin");

        Assert.Equal(EventImportJobStatus.Completed, job().Status);
        Assert.NotNull(job().CompletedAtUtc);
    }

    [Fact]
    public async Task CommitAsync_JobCannotBeSaved_EventsStayCreatedAndJobIdIsNull()
    {
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event { Id = 1 });
        _jobRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EventImportJob>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db down"));

        var result = await CreateEventImportService().CommitAsync(new EventImportBatch { Rows = { CreateValidRow() } }, "admin");

        Assert.Equal(1, result.CreatedCount);
        Assert.Null(result.JobId);
    }

    [Fact]
    public async Task CommitAsync_EventsAndJobGetTheSameTimestampFromTimeProvider()
    {
        Event? created = null;
        _eventServiceMock.Setup(s => s.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(e => created = e)
            .ReturnsAsync(new Event { Id = 1 });
        var job = CaptureSavedJob();

        await CreateEventImportService().CommitAsync(new EventImportBatch { Rows = { CreateValidRow() } }, "admin");

        var expected = ImportTime.UtcDateTime;
        Assert.Equal((expected, expected), (created!.CreatedAt, created.UpdatedAt));
        Assert.Equal((expected, expected), (job().CreatedAtUtc, job().CompletedAtUtc));
    }

    // Captures the job passed to the repository and gives it id 42, as the database would.
    private Func<EventImportJob> CaptureSavedJob()
    {
        EventImportJob? saved = null;
        _jobRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EventImportJob>(), It.IsAny<CancellationToken>()))
            .Callback<EventImportJob, CancellationToken>((j, _) =>
            {
                j.Id = 42;
                saved = j;
            })
            .Returns(Task.CompletedTask);
        return () => saved ?? throw new InvalidOperationException("No job was saved.");
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}

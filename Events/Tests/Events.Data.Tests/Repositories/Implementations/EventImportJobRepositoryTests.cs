using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

// Excel import image jobs (GS-REQ-009, 9.3.3, 9.3.4).
public class EventImportJobRepositoryTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 21, 10, 0, 0, DateTimeKind.Utc);

    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly EventsDbContext _context;

    public EventImportJobRepositoryTests()
    {
        _context = InMemoryDbContextFactory.Create(_databaseName);
    }

    public void Dispose() => _context.Dispose();

    private EventImportJobRepository CreateRepository() => new(_context);

    private static EventImportJob CreateJob(EventImportJobStatus status, DateTime createdAtUtc, params EventImportImageOutcome[] outcomes)
    {
        var job = new EventImportJob { FileName = "events.xlsx", CreatedBy = "admin", CreatedAtUtc = createdAtUtc, Status = status };
        for (var i = 0; i < outcomes.Length; i++)
        {
            job.Items.Add(new EventImportJobItem { RowNumber = outcomes.Length - i + 1, EventName = $"Row {i}", Outcome = outcomes[i] });
        }

        return job;
    }

    [Fact]
    public async Task AddAsync_SavesJobWithItems_ReadBackOrderedByRow()
    {
        var job = CreateJob(EventImportJobStatus.Queued, Now, EventImportImageOutcome.Pending, EventImportImageOutcome.NoImage);

        await CreateRepository().AddAsync(job);

        using var readContext = InMemoryDbContextFactory.Create(_databaseName);
        var saved = await new EventImportJobRepository(readContext).GetWithItemsAsync(job.Id);
        Assert.NotNull(saved);
        Assert.Equal([2, 3], saved!.Items.Select(i => i.RowNumber));
    }

    [Fact]
    public async Task GetUnfinishedJobIdsAsync_ReturnsQueuedAndRunningOldestFirst()
    {
        var running = CreateJob(EventImportJobStatus.Running, Now.AddMinutes(-5));
        var completed = CreateJob(EventImportJobStatus.Completed, Now.AddMinutes(-10));
        var queued = CreateJob(EventImportJobStatus.Queued, Now.AddMinutes(-20));
        _context.EventImportJobs.AddRange(running, completed, queued);
        await _context.SaveChangesAsync();

        var ids = await CreateRepository().GetUnfinishedJobIdsAsync();

        Assert.Equal([queued.Id, running.Id], ids);
    }

    [Fact]
    public async Task StartAsync_QueuedJob_BecomesRunningAndKeepsFirstStartTime()
    {
        var job = CreateJob(EventImportJobStatus.Queued, Now);
        _context.EventImportJobs.Add(job);
        await _context.SaveChangesAsync();
        var repository = CreateRepository();

        Assert.True(await repository.StartAsync(job.Id, Now));
        Assert.True(await repository.StartAsync(job.Id, Now.AddHours(1)));

        Assert.Equal(EventImportJobStatus.Running, job.Status);
        Assert.Equal(Now, job.StartedAtUtc);
    }

    [Fact]
    public async Task StartAsync_CompletedJob_RunsAgainForRetry()
    {
        var job = CreateJob(EventImportJobStatus.Completed, Now);
        job.StartedAtUtc = Now;
        _context.EventImportJobs.Add(job);
        await _context.SaveChangesAsync();

        Assert.True(await CreateRepository().StartAsync(job.Id, Now.AddHours(1)));

        Assert.Equal(EventImportJobStatus.Running, job.Status);
        Assert.Equal(Now, job.StartedAtUtc);
    }

    [Fact]
    public async Task StartAsync_MissingJob_ReturnsFalse()
    {
        Assert.False(await CreateRepository().StartAsync(999, Now));
    }

    [Fact]
    public async Task GetPendingItemIdsAsync_OnlyPendingItemsOfThatJob()
    {
        var job = CreateJob(EventImportJobStatus.Running, Now,
            EventImportImageOutcome.Pending, EventImportImageOutcome.Downloaded, EventImportImageOutcome.Pending);
        var other = CreateJob(EventImportJobStatus.Queued, Now, EventImportImageOutcome.Pending);
        _context.EventImportJobs.AddRange(job, other);
        await _context.SaveChangesAsync();

        var ids = await CreateRepository().GetPendingItemIdsAsync(job.Id);

        var expected = job.Items.Where(i => i.Outcome == EventImportImageOutcome.Pending).OrderBy(i => i.RowNumber).Select(i => i.Id);
        Assert.Equal(expected, ids);
    }

    [Fact]
    public async Task GetItemForUpdateAsync_ThenSaveChanges_PersistsItem()
    {
        var job = CreateJob(EventImportJobStatus.Running, Now, EventImportImageOutcome.Pending);
        _context.EventImportJobs.Add(job);
        await _context.SaveChangesAsync();
        var repository = CreateRepository();

        var item = await repository.GetItemForUpdateAsync(job.Items.First().Id);
        item!.Outcome = EventImportImageOutcome.Failed;
        item.FailureReason = ImageFetchFailureReason.Expired;
        await repository.SaveChangesAsync();

        using var readContext = InMemoryDbContextFactory.Create(_databaseName);
        var saved = readContext.EventImportJobItems.Single();
        Assert.Equal(ImageFetchFailureReason.Expired, saved.FailureReason);
    }

    [Fact]
    public async Task CompleteAsync_SetsStatusAndTime()
    {
        var job = CreateJob(EventImportJobStatus.Running, Now);
        _context.EventImportJobs.Add(job);
        await _context.SaveChangesAsync();

        await CreateRepository().CompleteAsync(job.Id, Now.AddMinutes(3));

        Assert.Equal(EventImportJobStatus.Completed, job.Status);
        Assert.Equal(Now.AddMinutes(3), job.CompletedAtUtc);
    }

    [Fact]
    public async Task GetRecentAsync_NewestFirstLimited()
    {
        _context.EventImportJobs.AddRange(
            CreateJob(EventImportJobStatus.Completed, Now.AddDays(-2)),
            CreateJob(EventImportJobStatus.Completed, Now),
            CreateJob(EventImportJobStatus.Completed, Now.AddDays(-1)));
        await _context.SaveChangesAsync();

        var recent = await CreateRepository().GetRecentAsync(2);

        Assert.Equal([Now, Now.AddDays(-1)], recent.Select(j => j.CreatedAtUtc));
    }
}

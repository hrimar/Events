using System.Collections.Concurrent;
using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Import;
using Events.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Events.Services.Tests.Import;

// Copy of the import images inside the Confirm request (GS-REQ-009, 9.1, 9.3; owner decision instead of 9.3.4).
public class EventImportImageJobRunnerTests
{
    private const int JobId = 5;

    private readonly Mock<IEventImportJobRepository> _repository = new();
    private readonly Mock<IEventImportImageProcessor> _processor = new();
    private readonly ConcurrentDictionary<int, EventImportJobItem> _items = new();

    public EventImportImageJobRunnerTests()
    {
        _repository.Setup(r => r.StartAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _repository.Setup(r => r.GetItemForUpdateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => _items.GetValueOrDefault(id));
        _processor.Setup(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()))
            .Callback<EventImportJobItem, CancellationToken>((item, _) => item.Outcome = EventImportImageOutcome.Downloaded)
            .Returns(Task.CompletedTask);
    }

    private EventImportImageJobRunner CreateRunner(TimeSpan? timeBudget = null)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => _repository.Object);
        services.AddScoped(_ => _processor.Object);
        var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        var logger = NullLogger<EventImportImageJobRunner>.Instance;
        return timeBudget == null
            ? new EventImportImageJobRunner(scopeFactory, TimeProvider.System, logger)
            : new EventImportImageJobRunner(scopeFactory, TimeProvider.System, logger) { TimeBudget = timeBudget.Value };
    }

    private void GivenPendingItems(params int[] ids)
    {
        foreach (var id in ids)
        {
            _items[id] = new EventImportJobItem { Id = id, JobId = JobId, RowNumber = id, EventId = 100 + id };
        }

        _repository.Setup(r => r.GetPendingItemIdsAsync(JobId, It.IsAny<CancellationToken>())).ReturnsAsync(ids.ToList());
    }

    [Fact]
    public async Task RunAsync_ProcessesAndSavesEveryPendingItemThenCompletesJob()
    {
        GivenPendingItems(1, 2, 3, 4, 5, 6);

        var notProcessed = await CreateRunner().RunAsync(JobId);

        Assert.All(_items.Values, i => Assert.Equal(EventImportImageOutcome.Downloaded, i.Outcome));
        _processor.Verify(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()), Times.Exactly(6));
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(6));
        _repository.Verify(r => r.CompleteAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(6, notProcessed); // the mock keeps returning the same ids; the real query returns only Pending rows
    }

    [Fact]
    public async Task RunAsync_RunsAtMostFourItemsAtOnce()
    {
        GivenPendingItems(Enumerable.Range(1, 12).ToArray());
        var running = 0;
        var maxRunning = 0;
        _processor.Setup(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                var now = Interlocked.Increment(ref running);
                lock (_items)
                {
                    maxRunning = Math.Max(maxRunning, now);
                }

                await Task.Delay(20);
                Interlocked.Decrement(ref running);
            });

        await CreateRunner().RunAsync(JobId);

        Assert.InRange(maxRunning, 2, EventImportImageJobRunner.MaxParallelDownloads);
    }

    [Fact]
    public async Task RunAsync_TimeBudgetReached_StopsCompletesJobAndReportsRemainingRows()
    {
        _items[1] = new EventImportJobItem { Id = 1, JobId = JobId, RowNumber = 1, EventId = 101 };
        _repository.SetupSequence(r => r.GetPendingItemIdsAsync(JobId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([1])
            .ReturnsAsync([1]);
        _processor.Setup(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()))
            .Returns((EventImportJobItem _, CancellationToken token) => Task.Delay(Timeout.Infinite, token));

        var notProcessed = await CreateRunner(TimeSpan.FromMilliseconds(100)).RunAsync(JobId);

        Assert.Equal(1, notProcessed);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.CompleteAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_MissingJob_DoesNothing()
    {
        _repository.Setup(r => r.StartAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var notProcessed = await CreateRunner().RunAsync(JobId);

        Assert.Equal(0, notProcessed);
        _processor.VerifyNoOtherCalls();
        _repository.Verify(r => r.CompleteAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_OneItemThrows_OthersStillProcessedAndJobCompletes()
    {
        GivenPendingItems(1, 2, 3);
        _repository.Setup(r => r.GetItemForUpdateAsync(2, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));

        await CreateRunner().RunAsync(JobId);

        Assert.Equal(EventImportImageOutcome.Downloaded, _items[1].Outcome);
        Assert.Equal(EventImportImageOutcome.Downloaded, _items[3].Outcome);
        _repository.Verify(r => r.CompleteAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

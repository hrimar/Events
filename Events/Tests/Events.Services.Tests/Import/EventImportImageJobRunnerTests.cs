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

// Background run of one import image job (GS-REQ-009, 9.3.4).
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

    private EventImportImageJobRunner CreateRunner()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => _repository.Object);
        services.AddScoped(_ => _processor.Object);
        var provider = services.BuildServiceProvider();
        return new EventImportImageJobRunner(
            provider.GetRequiredService<IServiceScopeFactory>(), TimeProvider.System, NullLogger<EventImportImageJobRunner>.Instance);
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
    public async Task RunAsync_ProcessesAndSavesEveryPendingItemInRowOrderThenCompletesJob()
    {
        GivenPendingItems(1, 2, 3, 4, 5, 6);
        var order = new List<int>();
        _processor.Setup(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()))
            .Callback<EventImportJobItem, CancellationToken>((item, _) =>
            {
                order.Add(item.Id);
                item.Outcome = EventImportImageOutcome.Downloaded;
            })
            .Returns(Task.CompletedTask);

        await CreateRunner().RunAsync(JobId, CancellationToken.None);

        Assert.Equal([1, 2, 3, 4, 5, 6], order);
        Assert.All(_items.Values, i => Assert.Equal(EventImportImageOutcome.Downloaded, i.Outcome));
        _processor.Verify(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()), Times.Exactly(6));
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(6));
        _repository.Verify(r => r.CompleteAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_JobMissingOrCompleted_DoesNothing()
    {
        _repository.Setup(r => r.StartAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await CreateRunner().RunAsync(JobId, CancellationToken.None);

        _processor.VerifyNoOtherCalls();
        _repository.Verify(r => r.CompleteAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_OneItemThrows_OthersStillProcessedAndJobCompletes()
    {
        GivenPendingItems(1, 2, 3);
        _repository.Setup(r => r.GetItemForUpdateAsync(2, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));

        await CreateRunner().RunAsync(JobId, CancellationToken.None);

        Assert.Equal(EventImportImageOutcome.Downloaded, _items[1].Outcome);
        Assert.Equal(EventImportImageOutcome.Downloaded, _items[3].Outcome);
        _repository.Verify(r => r.CompleteAsync(JobId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAsync_Shutdown_StopsWithoutCompletingJob()
    {
        GivenPendingItems(1, 2, 3, 4, 5, 6, 7, 8);
        using var cts = new CancellationTokenSource();
        _processor.Setup(p => p.ProcessAsync(It.IsAny<EventImportJobItem>(), It.IsAny<CancellationToken>()))
            .Returns(async (EventImportJobItem _, CancellationToken token) =>
            {
                cts.Cancel();
                await Task.Delay(Timeout.Infinite, token);
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateRunner().RunAsync(JobId, cts.Token));

        _repository.Verify(r => r.CompleteAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

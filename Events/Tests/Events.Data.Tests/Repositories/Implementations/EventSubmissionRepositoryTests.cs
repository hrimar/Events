using Events.Data.Context;
using Events.Data.Repositories.Implementations;
using Events.Data.Tests.TestSupport;
using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Tests.Repositories.Implementations;

// Organizer "Add event" submissions (GS-REQ-011, 11.4, 11.6).
public class EventSubmissionRepositoryTests : IDisposable
{
    private static readonly DateTime Now = new(2026, 11, 28, 10, 0, 0, DateTimeKind.Utc);

    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly EventsDbContext _context;

    public EventSubmissionRepositoryTests()
    {
        _context = InMemoryDbContextFactory.Create(_databaseName);
        _context.Users.AddRange(
            new User { Id = "u1", UserName = "one@example.com", Email = "one@example.com" },
            new User { Id = "u2", UserName = "two@example.com", Email = "two@example.com" });
        _context.Categories.Add(new Category { Id = 1, Name = "Music" });
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    private EventSubmissionRepository CreateRepository() => new(_context);

    private static EventSubmission CreateSubmission(
        string userId, string name, DateTime submittedAtUtc,
        EventSubmissionStatus status = EventSubmissionStatus.Pending, params DateTime[] days)
    {
        var submission = new EventSubmission
        {
            UserId = userId,
            Name = name,
            CategoryId = 1,
            StartDate = days.Length > 0 ? days.Min() : new DateTime(2026, 12, 10),
            Description = new string('a', 60),
            ImageUrl = "/media/1200/abc.webp",
            ThumbnailUrl = "/media/480/abc.webp",
            OrganizerName = "Organizer",
            SubmittedAtUtc = submittedAtUtc,
            ConsentAtUtc = submittedAtUtc,
            Status = status
        };

        foreach (var day in days)
        {
            submission.Dates.Add(new EventSubmissionDate { Date = day });
        }

        return submission;
    }

    [Fact]
    public async Task AddAsync_SavesSubmissionWithDays_DetailsReadsThemInDateOrder()
    {
        var submission = CreateSubmission("u1", "Concert", Now, EventSubmissionStatus.Pending,
            new DateTime(2026, 12, 16), new DateTime(2026, 12, 10));

        await CreateRepository().AddAsync(submission);

        using var readContext = InMemoryDbContextFactory.Create(_databaseName);
        var saved = await new EventSubmissionRepository(readContext).GetDetailsAsync(submission.Id);
        Assert.NotNull(saved);
        Assert.Equal([new DateTime(2026, 12, 10), new DateTime(2026, 12, 16)], saved!.Dates.Select(d => d.Date));
        Assert.Equal("one@example.com", saved.User.Email);
        Assert.Equal("Music", saved.Category.Name);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyTheUsersSubmissions_NewestFirst()
    {
        _context.EventSubmissions.AddRange(
            CreateSubmission("u1", "Old", Now.AddDays(-3)),
            CreateSubmission("u2", "Other user", Now.AddDays(-2)),
            CreateSubmission("u1", "New", Now.AddDays(-1), EventSubmissionStatus.Rejected));
        await _context.SaveChangesAsync();

        var result = await CreateRepository().GetByUserAsync("u1");

        Assert.Equal(["New", "Old"], result.Select(s => s.Name));
    }

    [Fact]
    public async Task GetByStatusAsync_ReturnsOneStatus_OldestFirst_WithPaging()
    {
        _context.EventSubmissions.AddRange(
            CreateSubmission("u1", "Second", Now.AddHours(-2)),
            CreateSubmission("u1", "Approved", Now.AddHours(-5), EventSubmissionStatus.Approved),
            CreateSubmission("u2", "First", Now.AddHours(-3)),
            CreateSubmission("u2", "Third", Now.AddHours(-1)));
        await _context.SaveChangesAsync();

        var firstPage = await CreateRepository().GetByStatusAsync(EventSubmissionStatus.Pending, skip: 0, take: 2);
        var secondPage = await CreateRepository().GetByStatusAsync(EventSubmissionStatus.Pending, skip: 2, take: 2);

        Assert.Equal(["First", "Second"], firstPage.Select(s => s.Name));
        Assert.Equal(["Third"], secondPage.Select(s => s.Name));
    }

    [Fact]
    public async Task CountByStatusAsync_CountsEachStatus()
    {
        _context.EventSubmissions.AddRange(
            CreateSubmission("u1", "A", Now),
            CreateSubmission("u1", "B", Now),
            CreateSubmission("u2", "C", Now, EventSubmissionStatus.Spam));
        await _context.SaveChangesAsync();

        var counts = await CreateRepository().CountByStatusAsync();

        Assert.Equal(2, counts[EventSubmissionStatus.Pending]);
        Assert.Equal(1, counts[EventSubmissionStatus.Spam]);
        Assert.False(counts.ContainsKey(EventSubmissionStatus.Approved));
    }

    [Fact]
    public async Task GetForUpdateAsync_ChangesAreSaved()
    {
        var submission = CreateSubmission("u1", "Concert", Now, EventSubmissionStatus.Pending, new DateTime(2026, 12, 10));
        _context.EventSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        var repository = CreateRepository();
        var tracked = await repository.GetForUpdateAsync(submission.Id);
        tracked!.Status = EventSubmissionStatus.Rejected;
        tracked.RejectionReason = "Not in Sofia";
        tracked.Actions.Add(new EventSubmissionAction
        {
            Action = EventSubmissionActionType.Rejected,
            PerformedByUserId = "admin-id",
            PerformedBy = "admin",
            PerformedAtUtc = Now
        });
        await repository.SaveChangesAsync();

        using var readContext = InMemoryDbContextFactory.Create(_databaseName);
        var saved = await new EventSubmissionRepository(readContext).GetDetailsAsync(submission.Id);
        Assert.Equal(EventSubmissionStatus.Rejected, saved!.Status);
        Assert.Equal("Not in Sofia", saved.RejectionReason);
        Assert.Single(saved.Actions);
        Assert.Single(saved.Dates);
    }

    [Fact]
    public async Task GetDetailsAsync_UnknownId_ReturnsNull()
    {
        Assert.Null(await CreateRepository().GetDetailsAsync(999));
    }
}

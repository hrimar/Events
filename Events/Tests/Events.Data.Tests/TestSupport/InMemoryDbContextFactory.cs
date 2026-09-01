using Events.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.TestSupport;

// Each call gets its own uniquely-named InMemory database so tests never leak state into
// each other, even when run in parallel (xUnit runs test classes in parallel by default).
public static class InMemoryDbContextFactory
{
    public static EventsDbContext Create() => Create(Guid.NewGuid().ToString());

    // Overload for tests that need to read back through a *second*, untracked context instance
    // pointed at the same database name - e.g. to avoid EF's change-tracker "fixup" silently
    // pulling already-tracked related entities into a filtered .Include(...) result, which would
    // otherwise mask the query's actual filtering behavior (see SubCategoryRepositoryTests).
    public static EventsDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<EventsDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new EventsDbContext(options);
    }
}

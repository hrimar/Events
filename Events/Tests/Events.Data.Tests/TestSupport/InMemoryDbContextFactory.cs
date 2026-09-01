using Events.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Events.Data.Tests.TestSupport;

// Each call gets its own uniquely-named InMemory database so tests never leak state into
// each other, even when run in parallel (xUnit runs test classes in parallel by default).
public static class InMemoryDbContextFactory
{
    public static EventsDbContext Create()
    {
        var options = new DbContextOptionsBuilder<EventsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new EventsDbContext(options);
    }
}

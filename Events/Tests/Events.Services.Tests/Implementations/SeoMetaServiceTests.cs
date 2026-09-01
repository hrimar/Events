using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class SeoMetaServiceTests
{
    private readonly Mock<IPageSeoMetaRepository> _repositoryMock = new();
    private readonly Mock<ILogger<SeoMetaService>> _loggerMock = new();

    private SeoMetaService CreateSeoMetaService() => new(_repositoryMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetAllAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var pages = new[] { new PageSeoMeta { PageKey = "home" } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pages);

        var result = await CreateSeoMetaService().GetAllAsync();

        Assert.Equal(pages, result);
    }

    [Fact]
    public async Task GetByKeyAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var page = new PageSeoMeta { PageKey = "home" };
        _repositoryMock.Setup(r => r.GetByKeyAsync("home")).ReturnsAsync(page);

        var result = await CreateSeoMetaService().GetByKeyAsync("home");

        Assert.Equal(page, result);
    }

    [Fact]
    public async Task GetByKeyAsync_KeyNotFound_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByKeyAsync("unknown")).ReturnsAsync((PageSeoMeta?)null);

        var result = await CreateSeoMetaService().GetByKeyAsync("unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAllAsync_UpdatesAllPagesInRepository()
    {
        var pages = new[] { new PageSeoMeta { PageKey = "home" }, new PageSeoMeta { PageKey = "events" } };

        await CreateSeoMetaService().SaveAllAsync(pages);

        _repositoryMock.Verify(r => r.UpdateManyAsync(pages), Times.Once);
    }
}

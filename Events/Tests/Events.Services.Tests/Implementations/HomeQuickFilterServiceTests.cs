using Events.Data.Repositories.Interfaces;
using Events.Services.Caching;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Implementations;
using Events.Services.Models.Admin;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class HomeQuickFilterServiceTests
{
    private readonly Mock<IHomeQuickFilterRepository> _repositoryMock = new();
    private readonly Mock<IEventCacheInvalidator> _cacheInvalidatorMock = new();
    private readonly Mock<ILogger<HomeQuickFilterService>> _loggerMock = new();
    private readonly List<HomeQuickFilter> _filters = new();

    public HomeQuickFilterServiceTests()
    {
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(() => _filters.OrderBy(f => f.SortOrder).ToList());
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HomeQuickFilter>()))
            .Callback<HomeQuickFilter>(f => _filters.Add(f))
            .Returns(Task.CompletedTask);
    }

    private HomeQuickFilterService CreateService() => new(_repositoryMock.Object, _cacheInvalidatorMock.Object, _loggerMock.Object);

    private void AddFilters(int count, bool isActive = true)
    {
        for (var i = 1; i <= count; i++)
        {
            _filters.Add(new HomeQuickFilter
            {
                Id = i,
                LabelBg = $"БГ{i}",
                LabelEn = $"EN{i}",
                Type = HomeQuickFilterType.Free,
                SortOrder = i,
                IsActive = isActive
            });
        }
    }

    private static HomeQuickFilterInput CategoriesInput(params string[] values) =>
        new("Бизнес", "Business", HomeQuickFilterType.Categories, values, IsActive: true);

    [Fact]
    public async Task CreateAsync_Categories_NormalizesNamesAndAppendsAtTheEnd()
    {
        AddFilters(2);

        var result = await CreateService().CreateAsync(CategoriesInput(" business ", "CONFERENCES", "Business"));

        Assert.True(result.Succeeded);
        var created = _filters.Last();
        Assert.Equal(new[] { "Business", "Conferences" }, created.Values);
        Assert.Equal(3, created.SortOrder);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Once);
    }

    [Theory]
    [InlineData("Nonsense")]
    [InlineData("Undefined")]
    [InlineData("3")]
    public async Task CreateAsync_UnknownCategory_Fails(string category)
    {
        var result = await CreateService().CreateAsync(CategoriesInput(category));

        Assert.False(result.Succeeded);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HomeQuickFilter>()), Times.Never);
        _cacheInvalidatorMock.Verify(c => c.Invalidate(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_TagsWithoutValues_Fails()
    {
        var input = new HomeQuickFilterInput("За деца", "For kids", HomeQuickFilterType.Tags, new[] { " ", "" }, IsActive: true);

        var result = await CreateService().CreateAsync(input);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_Free_IgnoresValues()
    {
        var input = new HomeQuickFilterInput("Безплатни", "Free", HomeQuickFilterType.Free, new[] { "Music" }, IsActive: true);

        var result = await CreateService().CreateAsync(input);

        Assert.True(result.Succeeded);
        Assert.Empty(_filters.Single().Values);
    }

    [Fact]
    public async Task CreateAsync_ActiveWhenEightAreActive_Fails()
    {
        AddFilters(HomeQuickFilter.MaxActive);

        var result = await CreateService().CreateAsync(CategoriesInput("Music"));

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task CreateAsync_InactiveWhenEightAreActive_Succeeds()
    {
        AddFilters(HomeQuickFilter.MaxActive);

        var result = await CreateService().CreateAsync(CategoriesInput("Music") with { IsActive = false });

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task SetActiveAsync_NinthFilter_Fails()
    {
        AddFilters(HomeQuickFilter.MaxActive);
        _filters.Add(new HomeQuickFilter { Id = 99, LabelBg = "Х", LabelEn = "X", SortOrder = 99, IsActive = false });

        var result = await CreateService().SetActiveAsync(99, true);

        Assert.False(result.Succeeded);
        Assert.False(_filters.Single(f => f.Id == 99).IsActive);
    }

    [Fact]
    public async Task MoveAsync_Up_SwapsWithPreviousAndRenumbers()
    {
        AddFilters(3);

        var result = await CreateService().MoveAsync(3, -1);

        Assert.True(result.Succeeded);
        Assert.Equal(new[] { 1, 3, 2 }, _filters.OrderBy(f => f.SortOrder).Select(f => f.Id));
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MoveAsync_FirstUp_DoesNothing()
    {
        AddFilters(3);

        var result = await CreateService().MoveAsync(1, -1);

        Assert.True(result.Succeeded);
        Assert.Equal(new[] { 1, 2, 3 }, _filters.OrderBy(f => f.SortOrder).Select(f => f.Id));
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetActiveAsync_ReturnsAtMostEight()
    {
        _repositoryMock.Setup(r => r.GetActiveAsync())
            .ReturnsAsync(Enumerable.Range(1, 10).Select(i => new HomeQuickFilter { Id = i, IsActive = true }).ToList());

        var result = await CreateService().GetActiveAsync();

        Assert.Equal(HomeQuickFilter.MaxActive, result.Count);
    }
}

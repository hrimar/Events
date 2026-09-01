using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class UserFavoriteEventServiceTests
{
    private readonly Mock<IUserFavoriteEventRepository> _repositoryMock = new();
    private readonly Mock<ILogger<UserFavoriteEventService>> _loggerMock = new();

    private UserFavoriteEventService CreateUserFavoriteEventService() => new(_repositoryMock.Object, _loggerMock.Object);

    // ToggleFavoriteAsync

    [Fact]
    public async Task ToggleFavoriteAsync_NotYetFavorite_AddsAndReturnsTrue()
    {
        _repositoryMock.Setup(r => r.IsFavoriteAsync("user1", 1)).ReturnsAsync(false);

        var result = await CreateUserFavoriteEventService().ToggleFavoriteAsync("user1", 1);

        Assert.True(result);
        _repositoryMock.Verify(r => r.AddFavoriteAsync("user1", 1), Times.Once);
        _repositoryMock.Verify(r => r.RemoveFavoriteAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_AlreadyFavorite_RemovesAndReturnsFalse()
    {
        _repositoryMock.Setup(r => r.IsFavoriteAsync("user1", 1)).ReturnsAsync(true);

        var result = await CreateUserFavoriteEventService().ToggleFavoriteAsync("user1", 1);

        Assert.False(result);
        _repositoryMock.Verify(r => r.RemoveFavoriteAsync("user1", 1), Times.Once);
        _repositoryMock.Verify(r => r.AddFavoriteAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_RepositoryThrows_RethrowsSameException()
    {
        _repositoryMock.Setup(r => r.IsFavoriteAsync(It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new InvalidOperationException("db error"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateUserFavoriteEventService().ToggleFavoriteAsync("user1", 1));
    }

    // AddFavoriteAsync

    [Fact]
    public async Task AddFavoriteAsync_NotYetFavorite_ReturnsTrue()
    {
        _repositoryMock.Setup(r => r.AddFavoriteAsync("user1", 1)).ReturnsAsync(new UserFavoriteEvent { UserId = "user1", EventId = 1 });

        var result = await CreateUserFavoriteEventService().AddFavoriteAsync("user1", 1);

        Assert.True(result);
    }

    [Fact]
    public async Task AddFavoriteAsync_AlreadyFavorite_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.AddFavoriteAsync("user1", 1)).ReturnsAsync((UserFavoriteEvent?)null);

        var result = await CreateUserFavoriteEventService().AddFavoriteAsync("user1", 1);

        Assert.False(result);
    }

    // RemoveFavoriteAsync

    [Fact]
    public async Task RemoveFavoriteAsync_WasFavorite_ReturnsTrue()
    {
        _repositoryMock.Setup(r => r.RemoveFavoriteAsync("user1", 1)).ReturnsAsync(true);

        var result = await CreateUserFavoriteEventService().RemoveFavoriteAsync("user1", 1);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WasNotFavorite_ReturnsFalse()
    {
        _repositoryMock.Setup(r => r.RemoveFavoriteAsync("user1", 1)).ReturnsAsync(false);

        var result = await CreateUserFavoriteEventService().RemoveFavoriteAsync("user1", 1);

        Assert.False(result);
    }

    // IsFavoriteAsync / GetUserFavoritesAsync / GetFavoriteCountAsync - pure delegation

    [Fact]
    public async Task IsFavoriteAsync_DelegatesToRepositoryAndReturnsResult()
    {
        _repositoryMock.Setup(r => r.IsFavoriteAsync("user1", 1)).ReturnsAsync(true);

        var result = await CreateUserFavoriteEventService().IsFavoriteAsync("user1", 1);

        Assert.True(result);
    }

    [Fact]
    public async Task GetUserFavoritesAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var favorites = new[] { new UserFavoriteEvent { UserId = "user1", EventId = 1 } };
        _repositoryMock.Setup(r => r.GetUserFavoritesAsync("user1")).ReturnsAsync(favorites);

        var result = await CreateUserFavoriteEventService().GetUserFavoritesAsync("user1");

        Assert.Equal(favorites, result);
    }

    [Fact]
    public async Task GetFavoriteCountAsync_DelegatesToRepositoryAndReturnsResult()
    {
        _repositoryMock.Setup(r => r.GetFavoriteCountAsync("user1")).ReturnsAsync(3);

        var result = await CreateUserFavoriteEventService().GetFavoriteCountAsync("user1");

        Assert.Equal(3, result);
    }
}

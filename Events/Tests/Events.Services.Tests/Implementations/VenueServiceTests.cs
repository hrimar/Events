using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class VenueServiceTests
{
    private readonly Mock<IVenueRepository> _repositoryMock = new();
    private readonly Mock<ILogger<VenueService>> _loggerMock = new();

    private VenueService CreateVenueService() => new(_repositoryMock.Object, _loggerMock.Object);

    private static CanonicalVenue CreateVenue(int id = 1, string name = "NDK") =>
        new() { Id = id, Name = name, NameEn = name, Slug = "ndk", City = "Sofia" };

    // FindCanonicalVenueIdAsync

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindCanonicalVenueIdAsync_BlankLocation_ReturnsNullWithoutCallingRepository(string? rawLocation)
    {
        var result = await CreateVenueService().FindCanonicalVenueIdAsync(rawLocation);

        Assert.Null(result);
        _repositoryMock.Verify(r => r.FindByNormalizedAliasAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task FindCanonicalVenueIdAsync_ExactAliasMatch_ReturnsVenueIdWithoutCheckingAllVenues()
    {
        var venue = CreateVenue(id: 5);
        _repositoryMock.Setup(r => r.FindByNormalizedAliasAsync("ndk")).ReturnsAsync(venue);

        var result = await CreateVenueService().FindCanonicalVenueIdAsync("NDK");

        Assert.Equal(5, result);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task FindCanonicalVenueIdAsync_ContainsMatch_CreatesAliasAndReturnsVenueId()
    {
        // Arrange
        var venue = CreateVenue(id: 5, name: "NDK");
        _repositoryMock.Setup(r => r.FindByNormalizedAliasAsync(It.IsAny<string>())).ReturnsAsync((CanonicalVenue?)null);
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([venue]);

        // Act
        var result = await CreateVenueService().FindCanonicalVenueIdAsync("NDK, Zala 1");

        // Assert
        Assert.Equal(5, result);
        _repositoryMock.Verify(r => r.AddAliasAsync(It.Is<VenueAlias>(a =>
            a.CanonicalVenueId == 5 && a.AliasString == "NDK, Zala 1" && a.NormalizedString == "ndk zala 1")), Times.Once);
    }

    [Fact]
    public async Task FindCanonicalVenueIdAsync_NoMatch_ReturnsNullWithoutCreatingAlias()
    {
        _repositoryMock.Setup(r => r.FindByNormalizedAliasAsync(It.IsAny<string>())).ReturnsAsync((CanonicalVenue?)null);
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([CreateVenue(name: "Arena")]);

        var result = await CreateVenueService().FindCanonicalVenueIdAsync("Some Unknown Place");

        Assert.Null(result);
        _repositoryMock.Verify(r => r.AddAliasAsync(It.IsAny<VenueAlias>()), Times.Never);
    }

    [Fact]
    public async Task FindCanonicalVenueIdAsync_RepositoryThrows_ReturnsNullInsteadOfThrowing()
    {
        _repositoryMock.Setup(r => r.FindByNormalizedAliasAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateVenueService().FindCanonicalVenueIdAsync("NDK");

        Assert.Null(result);
    }

    // GenerateUniqueSlugAsync

    [Fact]
    public async Task GenerateUniqueSlugAsync_SlugNotTaken_ReturnsBaseSlug()
    {
        _repositoryMock.Setup(r => r.SlugExistsAsync("ndk")).ReturnsAsync(false);

        var result = await CreateVenueService().GenerateUniqueSlugAsync("NDK");

        Assert.Equal("ndk", result);
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_SlugAlreadyTaken_AppendsNumericSuffix()
    {
        // Arrange: "ndk" and "ndk-2" are taken, "ndk-3" is free
        _repositoryMock.Setup(r => r.SlugExistsAsync("ndk")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.SlugExistsAsync("ndk-2")).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.SlugExistsAsync("ndk-3")).ReturnsAsync(false);

        // Act
        var result = await CreateVenueService().GenerateUniqueSlugAsync("NDK");

        // Assert
        Assert.Equal("ndk-3", result);
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_BlankSlug_FallsBackToVenue()
    {
        _repositoryMock.Setup(r => r.SlugExistsAsync("venue")).ReturnsAsync(false);

        var result = await CreateVenueService().GenerateUniqueSlugAsync("!!!");

        Assert.Equal("venue", result);
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_RepositoryThrows_ReturnsBaseSlugInsteadOfThrowing()
    {
        _repositoryMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("db error"));

        var result = await CreateVenueService().GenerateUniqueSlugAsync("NDK");

        Assert.Equal("ndk", result);
    }

    // AddAliasAsync

    [Fact]
    public async Task AddAliasAsync_AliasCannotBeNormalized_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => CreateVenueService().AddAliasAsync(1, "   "));
    }

    [Fact]
    public async Task AddAliasAsync_ValidAlias_PersistsNormalizedAlias()
    {
        var expectedAlias = new VenueAlias { CanonicalVenueId = 1, AliasString = "NDK", NormalizedString = "ndk" };
        _repositoryMock
            .Setup(r => r.AddAliasAsync(It.Is<VenueAlias>(a => a.CanonicalVenueId == 1 && a.NormalizedString == "ndk")))
            .ReturnsAsync(expectedAlias);

        var result = await CreateVenueService().AddAliasAsync(1, "  NDK  ");

        Assert.Equal(expectedAlias, result);
    }

    // Pure delegation methods

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var venue = CreateVenue();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(venue);

        var result = await CreateVenueService().GetByIdAsync(1);

        Assert.Equal(venue, result);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        await CreateVenueService().DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAliasAsync_DelegatesToRepository()
    {
        await CreateVenueService().DeleteAliasAsync(9);

        _repositoryMock.Verify(r => r.DeleteAliasAsync(9), Times.Once);
    }
}

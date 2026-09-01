using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Models.Enums;
using Events.Services.Implementations;
using Moq;

namespace Events.Services.Tests.Implementations;

// SubCategoryService has no logic beyond delegation - these tests guard against
// a future change silently introducing logic (validation, caching, etc.) without tests.
public class SubCategoryServiceTests
{
    private readonly Mock<ISubCategoryRepository> _repositoryMock = new();

    private SubCategoryService CreateSubCategoryService() => new(_repositoryMock.Object);

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategory = new SubCategory { Id = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subCategory);

        var result = await CreateSubCategoryService().GetByIdAsync(1);

        Assert.Equal(subCategory, result);
    }

    [Fact]
    public async Task GetAllAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategories = new[] { new SubCategory { Id = 1 }, new SubCategory { Id = 2 } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(subCategories);

        var result = await CreateSubCategoryService().GetAllAsync();

        Assert.Equal(subCategories, result);
    }

    [Fact]
    public async Task GetByCategoryAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategories = new[] { new SubCategory { Id = 1 } };
        _repositoryMock.Setup(r => r.GetByCategoryAsync(EventCategory.Music)).ReturnsAsync(subCategories);

        var result = await CreateSubCategoryService().GetByCategoryAsync(EventCategory.Music);

        Assert.Equal(subCategories, result);
    }

    [Fact]
    public async Task GetByEnumValueAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategory = new SubCategory { Id = 1 };
        _repositoryMock.Setup(r => r.GetByEnumValueAsync(EventCategory.Music, 3)).ReturnsAsync(subCategory);

        var result = await CreateSubCategoryService().GetByEnumValueAsync(EventCategory.Music, 3);

        Assert.Equal(subCategory, result);
    }

    [Fact]
    public async Task GetByNameAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategory = new SubCategory { Id = 1 };
        _repositoryMock.Setup(r => r.GetByNameAsync(EventCategory.Music, "Rock")).ReturnsAsync(subCategory);

        var result = await CreateSubCategoryService().GetByNameAsync(EventCategory.Music, "Rock");

        Assert.Equal(subCategory, result);
    }

    [Fact]
    public async Task CreateAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategory = new SubCategory { Id = 1 };
        _repositoryMock.Setup(r => r.AddAsync(subCategory)).ReturnsAsync(subCategory);

        var result = await CreateSubCategoryService().CreateAsync(subCategory);

        Assert.Equal(subCategory, result);
    }

    [Fact]
    public async Task UpdateAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var subCategory = new SubCategory { Id = 1 };
        _repositoryMock.Setup(r => r.UpdateAsync(subCategory)).ReturnsAsync(subCategory);

        var result = await CreateSubCategoryService().UpdateAsync(subCategory);

        Assert.Equal(subCategory, result);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToRepository()
    {
        await CreateSubCategoryService().DeleteAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}

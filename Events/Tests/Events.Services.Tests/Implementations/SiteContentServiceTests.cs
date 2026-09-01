using Events.Data.Repositories.Interfaces;
using Events.Models.Entities;
using Events.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace Events.Services.Tests.Implementations;

public class SiteContentServiceTests
{
    private readonly Mock<ISiteContentRepository> _repositoryMock = new();
    private readonly Mock<ILogger<SiteContentService>> _loggerMock = new();

    private SiteContentService CreateSiteContentService() => new(_repositoryMock.Object, _loggerMock.Object);

    [Fact]
    public async Task GetAsync_DelegatesToRepositoryAndReturnsResult()
    {
        var siteContent = new SiteContent();
        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(siteContent);

        var result = await CreateSiteContentService().GetAsync();

        Assert.Equal(siteContent, result);
    }

    [Fact]
    public async Task UpdateHeroAsync_UpdatesAllHeroFieldsAndPersists()
    {
        var siteContent = new SiteContent();
        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(siteContent);
        _repositoryMock.Setup(r => r.UpdateAsync(siteContent)).ReturnsAsync(siteContent);

        await CreateSiteContentService().UpdateHeroAsync("ЗаглавиеБГ", "TitleEN", "ПодзаглавиеБГ", "SubtitleEN");

        Assert.Equal("ЗаглавиеБГ", siteContent.HeroTitleBg);
        Assert.Equal("TitleEN", siteContent.HeroTitleEn);
        Assert.Equal("ПодзаглавиеБГ", siteContent.HeroSubtitleBg);
        Assert.Equal("SubtitleEN", siteContent.HeroSubtitleEn);
        _repositoryMock.Verify(r => r.UpdateAsync(siteContent), Times.Once);
    }

    [Fact]
    public async Task UpdateAboutUsAsync_SanitizesHtmlBeforePersisting()
    {
        // Arrange
        var siteContent = new SiteContent();
        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(siteContent);
        _repositoryMock.Setup(r => r.UpdateAsync(siteContent)).ReturnsAsync(siteContent);

        // Act - <script> is not in the sanitizer's allowed-tags list and must be stripped
        await CreateSiteContentService().UpdateAboutUsAsync("<p>Текст</p><script>alert('x')</script>", "<p>Text</p><script>alert('x')</script>");

        // Assert
        Assert.Equal("<p>Текст</p>", siteContent.AboutUsContentBg);
        Assert.Equal("<p>Text</p>", siteContent.AboutUsContentEn);
        _repositoryMock.Verify(r => r.UpdateAsync(siteContent), Times.Once);
    }
}

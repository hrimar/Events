using Events.Models.Entities;
using Events.Web.Localization;

namespace Events.Web.Tests.Localization;

public class EventLocalizationExtensionsTests
{
    private static Event CreateEvent() => new()
    {
        Name = "Лебедово езеро",
        Description = "Балет в три действия",
        Location = "ул. Гео Милев 8",
        City = "София"
    };

    [Theory]
    [InlineData(null, false)]
    [InlineData("  ", false)]
    [InlineData("Swan Lake", true)]
    public void HasEnglishTitle_RequiresANonEmptyNameEn(string? nameEn, bool expected)
    {
        var eventEntity = CreateEvent();
        eventEntity.NameEn = nameEn;

        Assert.Equal(expected, eventEntity.HasEnglishTitle());
    }

    [Fact]
    public void LocalizedName_English_UsesNameEn()
    {
        var eventEntity = CreateEvent();
        eventEntity.NameEn = "Swan Lake";

        Assert.Equal("Swan Lake", eventEntity.LocalizedName(isEnglish: true));
        Assert.Equal("Лебедово езеро", eventEntity.LocalizedName(isEnglish: false));
    }

    [Fact]
    public void LocalizedName_EnglishWithoutNameEn_FallsBackToTheBulgarianTitle()
    {
        Assert.Equal("Лебедово езеро", CreateEvent().LocalizedName(isEnglish: true));
    }

    [Fact]
    public void LocalizedDescription_EnglishWithoutDescriptionEn_FallsBackToTheBulgarianDescription()
    {
        var eventEntity = CreateEvent();

        Assert.Equal("Балет в три действия", eventEntity.LocalizedDescription(isEnglish: true));
    }

    [Fact]
    public void LocalizedDescription_EnglishWithEnglishTitle_NeverShowsTheBulgarianDescription()
    {
        var eventEntity = CreateEvent();
        eventEntity.NameEn = "Swan Lake";

        Assert.Null(eventEntity.LocalizedDescription(isEnglish: true));

        eventEntity.DescriptionEn = "A ballet in three acts";
        Assert.Equal("A ballet in three acts", eventEntity.LocalizedDescription(isEnglish: true));
    }

    [Fact]
    public void LocalizedLocation_Bulgarian_KeepsTheFreeTextEvenWithAVenue()
    {
        var eventEntity = CreateEvent();
        eventEntity.CanonicalVenue = new CanonicalVenue { Name = "НДК", NameEn = "National Palace of Culture" };

        Assert.Equal("ул. Гео Милев 8", eventEntity.LocalizedLocation(isEnglish: false));
    }

    [Fact]
    public void LocalizedLocation_EnglishWithVenue_UsesTheEnglishVenueName()
    {
        var eventEntity = CreateEvent();
        eventEntity.CanonicalVenue = new CanonicalVenue { Name = "НДК", NameEn = "National Palace of Culture" };

        Assert.Equal("National Palace of Culture", eventEntity.LocalizedLocation(isEnglish: true));
    }

    [Fact]
    public void LocalizedLocation_EnglishWithoutVenue_TransliteratesTheFreeText()
    {
        Assert.Equal("ul. Geo Milev 8", CreateEvent().LocalizedLocation(isEnglish: true));
    }

    [Fact]
    public void LocalizedCity_English_IsTransliterated()
    {
        Assert.Equal("Sofia", CreateEvent().LocalizedCity(isEnglish: true));
        Assert.Equal("София", CreateEvent().LocalizedCity(isEnglish: false));
    }
}

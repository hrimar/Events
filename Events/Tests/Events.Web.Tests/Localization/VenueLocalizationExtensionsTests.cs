using Events.Models.Entities;
using Events.Web.Localization;

namespace Events.Web.Tests.Localization;

public class VenueLocalizationExtensionsTests
{
    [Fact]
    public void LocalizedName_Bulgarian_UsesName()
    {
        var venue = new CanonicalVenue { Name = "НДК", NameEn = "National Palace of Culture" };

        Assert.Equal("НДК", venue.LocalizedName(isEnglish: false));
    }

    [Fact]
    public void LocalizedName_English_UsesNameEn()
    {
        var venue = new CanonicalVenue { Name = "НДК", NameEn = "National Palace of Culture" };

        Assert.Equal("National Palace of Culture", venue.LocalizedName(isEnglish: true));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void LocalizedName_EnglishWithEmptyNameEn_TransliteratesTheWholeBulgarianName(string nameEn)
    {
        var venue = new CanonicalVenue { Name = "Софийска опера и балет", NameEn = nameEn };

        Assert.Equal("Sofiyska opera i balet", venue.LocalizedName(isEnglish: true));
    }

    [Fact]
    public void LocalizeAddress_TransliteratesOnlyOnEnglishPages()
    {
        Assert.Equal("bul. Bulgaria 1", VenueLocalizationExtensions.LocalizeAddress("бул. България 1", isEnglish: true));
        Assert.Equal("бул. България 1", VenueLocalizationExtensions.LocalizeAddress("бул. България 1", isEnglish: false));
        Assert.Null(VenueLocalizationExtensions.LocalizeAddress(null, isEnglish: true));
    }
}

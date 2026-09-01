using Events.Services.Helpers;

namespace Events.Services.Tests.Helpers;

public class VenueNormalizerTests
{
    // Normalize

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_NullOrWhitespace_ReturnsNull(string? input)
    {
        Assert.Null(VenueNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_RemovesPunctuationAndCollapsesWhitespaceAndLowercases()
    {
        var result = VenueNormalizer.Normalize("  NDK,  Zala  1!  ");

        Assert.Equal("ndk zala 1", result);
    }

    // GenerateSlug

    [Fact]
    public void GenerateSlug_EmptyName_ReturnsEmptyString()
    {
        Assert.Equal(string.Empty, VenueNormalizer.GenerateSlug("   "));
    }

    [Fact]
    public void GenerateSlug_LatinName_ReplacesSpacesWithHyphens()
    {
        Assert.Equal("national-palace-of-culture", VenueNormalizer.GenerateSlug("National Palace of Culture"));
    }

    [Fact]
    public void GenerateSlug_CyrillicName_TransliteratesToLatin()
    {
        Assert.Equal("ndk", VenueNormalizer.GenerateSlug("НДК"));
    }

    [Fact]
    public void GenerateSlug_NameWithPunctuation_RemovesPunctuation()
    {
        Assert.Equal("zala-1", VenueNormalizer.GenerateSlug("Zala #1!"));
    }
}

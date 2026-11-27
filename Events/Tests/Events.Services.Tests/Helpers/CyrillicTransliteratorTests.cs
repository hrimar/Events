using Events.Services.Helpers;

namespace Events.Services.Tests.Helpers;

public class CyrillicTransliteratorTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Swan Lake", false)]
    [InlineData("Sofia Christmas Fest на Лед", true)]
    [InlineData("Ѐ", true)]
    public void ContainsCyrillic_DetectsAnyCyrillicLetter(string? text, bool expected)
    {
        Assert.Equal(expected, CyrillicTransliterator.ContainsCyrillic(text));
    }

    [Theory]
    [InlineData("НДК", "NDK")]
    [InlineData("Зала България", "Zala Bulgaria")]
    [InlineData("София", "Sofia")]
    [InlineData("Щастие", "Shtastie")]
    [InlineData("ЖАР", "ZHAR")]
    [InlineData("Ж", "Zh")]
    [InlineData("ЖИВОТ И ЩАСТИЕ", "ZHIVOT I SHTASTIE")]
    [InlineData("Юлия Ангелова", "Yulia Angelova")]
    [InlineData("пъпеш", "papesh")]
    [InlineData("Шопски ястия", "Shopski yastia")]
    [InlineData("Тиян", "Tiyan")]
    public void ToLatin_FollowsTheBulgarianTransliterationAct(string text, string expected)
    {
        Assert.Equal(expected, CyrillicTransliterator.ToLatin(text));
    }

    // Established spellings (GS-REQ-010, 10.4)

    [Theory]
    [InlineData("България", "Bulgaria")]
    [InlineData("БЪЛГАРИЯ", "BULGARIA")]
    [InlineData("българия", "Bulgaria")]
    [InlineData("Галерия „България“", "Galeria „Bulgaria“")]
    [InlineData("Българията", "Balgariyata")]
    [InlineData("Български", "Balgarski")]
    public void ToLatin_CountryName_UsesTheEstablishedSpellingOnlyAsAWholeWord(string text, string expected)
    {
        Assert.Equal(expected, CyrillicTransliterator.ToLatin(text));
    }

    [Fact]
    public void ToLatin_MixedText_KeepsLatinPunctuationAndDigits()
    {
        var result = CyrillicTransliterator.ToLatin("GVT FEST | МАЙСТОРСКИ КЛАС |Drink like an Italian- 2026");

        Assert.Equal("GVT FEST | MAYSTORSKI KLAS |Drink like an Italian- 2026", result);
    }

    [Theory]
    [InlineData("Ёлка Київ Ђорђе", "Yolka Kiyiv Djordje")]
    [InlineData("Ӂ test", " test")]
    public void ToLatin_NonBulgarianLetters_AreMappedOrDropped_NeverLeftCyrillic(string text, string expected)
    {
        var result = CyrillicTransliterator.ToLatin(text);

        Assert.Equal(expected, result);
        Assert.False(CyrillicTransliterator.ContainsCyrillic(result));
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("Swan Lake", "Swan Lake")]
    public void ToLatin_NoCyrillic_ReturnsTheText(string? text, string expected)
    {
        Assert.Equal(expected, CyrillicTransliterator.ToLatin(text));
    }
}

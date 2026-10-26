using Events.Web.Services;

namespace Events.Web.Tests.Services;

public class EventExcerptBuilderTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Build_EmptyDescription_IsNull(string? description)
    {
        Assert.Null(EventExcerptBuilder.Build(description, "Черна комедия"));
    }

    [Theory]
    [InlineData("25 септември ЧЕРНА КОМЕДИЯ")]
    [InlineData("Черна комедия - петък, 25.09.2026, 19:00")]
    [InlineData("BLACK COMEDY 25 September")]
    public void Build_OnlyTitleAndDate_IsNull(string description)
    {
        var eventName = description.StartsWith("BLACK") ? "Black comedy" : "Черна комедия";

        Assert.Null(EventExcerptBuilder.Build(description, eventName));
    }

    [Fact]
    public void Build_ShortDescription_IsKeptWhole_WithoutHtmlAndExtraSpaces()
    {
        var result = EventExcerptBuilder.Build("<p>Симфоничен  концерт&nbsp;в голямата\nзала на операта.</p>", "Страст и съдба");

        Assert.Equal("Симфоничен концерт в голямата зала на операта.", result);
    }

    [Fact]
    public void Build_LongDescription_IsCutAtTheLastSentenceThatFits()
    {
        var description = "Норвежкият стоунър рок идва с турнето Year of the Horse и представя новия си албум. " +
                          "Подгрява Godsleep. Билетите са ограничени, а вратите отварят в 19:00 часа вечерта.";

        var result = EventExcerptBuilder.Build(description, "Slomosa (NOR) на живо");

        Assert.Equal("Норвежкият стоунър рок идва с турнето Year of the Horse и представя новия си албум. Подгрява Godsleep.", result);
    }

    [Fact]
    public void Build_LongDescriptionWithoutSentenceEnd_IsCutAtAWordWithEllipsis()
    {
        var description = string.Join(' ', Enumerable.Repeat("думата", 40));

        var result = EventExcerptBuilder.Build(description, "Събитие")!;

        Assert.EndsWith("…", result);
        Assert.True(result.Length <= EventExcerptBuilder.MaxLength + 1);
        Assert.EndsWith("думата…", result);
    }
}

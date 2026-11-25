using System.Globalization;
using Events.Models.Entities;
using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class EventViewModelTests
{
    private static Event CreateEvent() => new()
    {
        Id = 1,
        Name = "BG_NAME",
        NameEn = "Swan Lake",
        Description = "BG_DESCRIPTION",
        DescriptionEn = "A ballet",
        Location = "NDK",
        City = "Sofia",
        Date = new DateTime(2026, 6, 15)
    };

    [Theory]
    [InlineData("en", "Swan Lake", "A ballet")]
    [InlineData("bg", "BG_NAME", "BG_DESCRIPTION")]
    public void FromEntity_UsesTheTextsOfTheRequestLanguage(string culture, string expectedName, string expectedDescription)
    {
        var previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            var model = EventViewModel.FromEntity(CreateEvent(), new DateTime(2026, 6, 1));

            Assert.Equal(expectedName, model.Name);
            Assert.Equal(expectedDescription, model.Description);
            Assert.Equal("BG_NAME", model.BulgarianName);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}

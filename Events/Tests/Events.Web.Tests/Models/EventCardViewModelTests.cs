using Events.Web.Models;

namespace Events.Web.Tests.Models;

public class EventCardViewModelTests
{
    private static readonly DateTime Friday = new(2026, 9, 25);

    [Fact]
    public void FromEvent_MapsCardFields()
    {
        var card = EventCardViewModel.FromEvent(new EventViewModel
        {
            Id = 1593,
            Name = "Едни момичета",
            Location = "Народен театър · Камерна сцена",
            Date = Friday,
            StartTime = new TimeSpan(19, 0, 0),
            ImageUrl = "/media/480/1593.webp",
            ThumbnailUrl = "/media/thumb/1593.webp",
            IsFree = true,
            CategoryName = "Theatre",
            SubCategoryName = "Drama"
        });

        Assert.Equal(1593, card.Id);
        Assert.Equal("Едни момичета", card.Name);
        Assert.Equal("Народен театър · Камерна сцена", card.Venue);
        Assert.Equal("/media/480/1593.webp", card.ImageUrl);
        Assert.Equal("/media/thumb/1593.webp", card.ThumbnailUrl);
        Assert.True(card.IsFree);
        Assert.Equal("Theatre", card.CategoryName);
        Assert.Equal("Drama", card.SubCategoryName);
        Assert.Equal(Friday, card.Date);
        Assert.Equal("19:00", card.TimeText);
        Assert.True(card.HasKnownTime);
    }

    [Fact]
    public void FromEvent_TimeStoredInDate_IsUsedForTheBadge()
    {
        var card = EventCardViewModel.FromEvent(new EventViewModel { Id = 1, Name = "A", Date = Friday.AddHours(21).AddMinutes(30) });

        Assert.Equal("21:30", card.TimeText);
        Assert.Equal(Friday, card.Date);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FromEvent_UnknownTime_HasNoTimeBadge(bool storedAsMidnight)
    {
        var card = EventCardViewModel.FromEvent(new EventViewModel
        {
            Id = 1,
            Name = "A",
            Date = Friday,
            StartTime = storedAsMidnight ? TimeSpan.Zero : null
        });

        Assert.Null(card.TimeText);
        Assert.False(card.HasKnownTime);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void FromEvent_WithoutImage_UsesDefaultImage(string? imageUrl)
    {
        var card = EventCardViewModel.FromEvent(new EventViewModel { Id = 1, Name = "A", Date = Friday, ImageUrl = imageUrl });

        Assert.Equal("/images/default_event_image.jpeg", card.ImageUrl);
    }

    [Fact]
    public void FromEvent_ShowsDateByDefaultAndCopiesThePastFlag()
    {
        var card = EventCardViewModel.FromEvent(new EventViewModel { Id = 1, Name = "A", Date = Friday, IsPastEvent = true });

        Assert.True(card.ShowDate);
        Assert.True(card.IsPast);
    }

    [Fact]
    public void FromEvents_UnderDayHeading_HidesTheDate()
    {
        var cards = EventCardViewModel.FromEvents(new[] { new EventViewModel { Id = 1, Name = "A", Date = Friday } }, showDate: false);

        Assert.False(Assert.Single(cards).ShowDate);
    }

    [Fact]
    public void FromEvents_KeepsOneCardPerPerformanceInInputOrder()
    {
        var events = new[]
        {
            new EventViewModel { Id = 1, Name = "Едни момичета", Location = "Народен театър", Date = Friday, StartTime = new TimeSpan(19, 0, 0) },
            new EventViewModel { Id = 2, Name = "Едни момичета", Location = "Народен театър", Date = Friday, StartTime = new TimeSpan(21, 30, 0) }
        };

        var cards = EventCardViewModel.FromEvents(events);

        Assert.Equal(new[] { 1, 2 }, cards.Select(c => c.Id));
        Assert.Equal(new[] { "19:00", "21:30" }, cards.Select(c => c.TimeText));
    }
}

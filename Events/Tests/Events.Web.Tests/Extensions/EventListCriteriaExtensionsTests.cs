using Events.Models.Enums;
using Events.Models.Queries;
using Events.Web.Extensions;

namespace Events.Web.Tests.Extensions;

public class EventListCriteriaExtensionsTests
{
    [Fact]
    public void ToRouteValues_AlwaysIncludesPageSizeSortByAndSortOrder()
    {
        var criteria = new EventListCriteria { Page = 2, PageSize = 20, SortBy = "date", SortOrder = "asc" };

        var result = criteria.ToRouteValues();

        Assert.Equal(2, result["page"]);
        Assert.Equal(20, result["pageSize"]);
        Assert.Equal("date", result["sortBy"]);
        Assert.Equal("asc", result["sortOrder"]);
    }

    [Fact]
    public void ToRouteValues_ExplicitPageOverridesCriteriaPage()
    {
        var criteria = new EventListCriteria { Page = 2 };

        var result = criteria.ToRouteValues(page: 5);

        Assert.Equal(5, result["page"]);
    }

    [Fact]
    public void ToRouteValues_OptionalFieldsUnset_AreOmittedFromRouteValues()
    {
        var criteria = new EventListCriteria();

        var result = criteria.ToRouteValues();

        Assert.False(result.ContainsKey("search"));
        Assert.False(result.ContainsKey("status"));
        Assert.False(result.ContainsKey("categoryId"));
        Assert.False(result.ContainsKey("subCategoryId"));
        Assert.False(result.ContainsKey("fromDate"));
        Assert.False(result.ContainsKey("toDate"));
        Assert.False(result.ContainsKey("createdAtFrom"));
        Assert.False(result.ContainsKey("createdAtTo"));
    }

    [Fact]
    public void ToRouteValues_BlankSearch_IsOmitted()
    {
        var criteria = new EventListCriteria { Search = "   " };

        var result = criteria.ToRouteValues();

        Assert.False(result.ContainsKey("search"));
    }

    [Fact]
    public void ToRouteValues_AllOptionalFieldsSet_AreIncludedWithExpectedFormatting()
    {
        var criteria = new EventListCriteria
        {
            Search = "jazz",
            Status = EventStatus.Published,
            CategoryId = 3,
            SubCategoryId = 7,
            FromDate = new DateTime(2026, 6, 1),
            ToDate = new DateTime(2026, 6, 30),
            CreatedAtFrom = new DateTime(2026, 1, 1),
            CreatedAtTo = new DateTime(2026, 1, 31)
        };

        var result = criteria.ToRouteValues();

        Assert.Equal("jazz", result["search"]);
        Assert.Equal((int)EventStatus.Published, result["status"]);
        Assert.Equal(3, result["categoryId"]);
        Assert.Equal(7, result["subCategoryId"]);
        Assert.Equal("2026-06-01", result["fromDate"]);
        Assert.Equal("2026-06-30", result["toDate"]);
        Assert.Equal("2026-01-01", result["createdAtFrom"]);
        Assert.Equal("2026-01-31", result["createdAtTo"]);
    }
}

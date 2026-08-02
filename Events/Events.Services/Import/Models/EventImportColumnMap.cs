namespace Events.Services.Import.Models;

/// <summary>
/// Maps the canonical fields the import mapper needs onto the actual header names found in an
/// uploaded file. Ship additional named instances (rather than a generic mapping UI) if a future
/// data provider uses a differently-shaped file.
/// </summary>
public class EventImportColumnMap
{
    public required string EventName { get; init; }
    public required string Date { get; init; }
    public required string StartTime { get; init; }
    public required string City { get; init; }
    public required string VenueLocation { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public required string SubCategory { get; init; }
    public required string Tags { get; init; }
    public required string ImageUrl { get; init; }
    public required string TicketUrl { get; init; }
    public required string FreeEventText { get; init; }
    public required string EventStatus { get; init; }
    public required string FeaturedEvent { get; init; }

    /// <summary>
    /// Column layout matching the reference .xlsx/.csv template provided by the admin:
    /// Event Name, Date, Start Time, City, Venue/Location, Description, Category, Subcategory,
    /// Tags (up to 3), Image URL, Ticket URL, This is a free event, Event Status, Featured Event.
    /// </summary>
    public static readonly EventImportColumnMap Default = new()
    {
        EventName = "Event Name",
        Date = "Date",
        StartTime = "Start Time",
        City = "City",
        VenueLocation = "Venue/Location",
        Description = "Description",
        Category = "Category",
        SubCategory = "Subcategory",
        Tags = "Tags (up to 3)",
        ImageUrl = "Image URL",
        TicketUrl = "Ticket URL",
        FreeEventText = "This is a free event",
        EventStatus = "Event Status",
        FeaturedEvent = "Featured Event (Display on homepage)",
    };
}

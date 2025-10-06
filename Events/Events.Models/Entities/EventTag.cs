namespace Events.Models.Entities;

public class EventTag
{
    public int EventId { get; set; }
    public int TagId { get; set; }

    public virtual Event Event { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
}

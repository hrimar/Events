namespace Events.Models.DTOs;

// The few columns needed to group a period's events by day and order them by time (GS-REQ-008, 8.2.6, 8.2.8),
// so the whole period can be counted and ordered without loading full events with their tags and venues.
public sealed record EventListingKeyDto(int Id, DateTime Date, TimeSpan? StartTime, string Name);

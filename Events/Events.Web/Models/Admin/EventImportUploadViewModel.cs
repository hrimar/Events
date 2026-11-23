using Events.Models.Entities;

namespace Events.Web.Models.Admin;

public class EventImportUploadViewModel
{
    public string? ErrorMessage { get; set; }

    // Latest imports with a link to their report (GS-REQ-009, 9.3.3).
    public List<EventImportJob> RecentJobs { get; set; } = new();
}

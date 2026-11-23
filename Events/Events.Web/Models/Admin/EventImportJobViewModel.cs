using Events.Models.Entities;
using Events.Services.Import;

namespace Events.Web.Models.Admin;

// Import report page (GS-REQ-009, 9.3.3-9.3.5).
public class EventImportJobViewModel
{
    public required EventImportJob Job { get; init; }

    public required EventImportReport Report { get; init; }
}

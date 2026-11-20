namespace Events.Models.Enums;

// Lifecycle of the background image job started by an Excel/CSV import (GS-REQ-009, 9.3.4).
public enum EventImportJobStatus
{
    // Events are saved; images are waiting for the background worker.
    Queued = 1,

    // The worker is processing the images (also left in this state if the app stopped mid-job,
    // so the worker resumes it on the next start).
    Running = 2,

    // Every row has a final outcome.
    Completed = 3
}

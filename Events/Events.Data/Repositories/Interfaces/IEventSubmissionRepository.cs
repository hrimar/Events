using Events.Models.Entities;
using Events.Models.Enums;

namespace Events.Data.Repositories.Interfaces;

// Organizer "Add event" submissions (GS-REQ-011, 11.4, 11.6, 11.7).
public interface IEventSubmissionRepository
{
    // Saves a new submission together with its days.
    Task AddAsync(EventSubmission submission, CancellationToken cancellationToken = default);

    // Submission with its days, category, subcategory, venue, user and admin history. Read-only.
    Task<EventSubmission?> GetDetailsAsync(int id, CancellationToken cancellationToken = default);

    // Tracked submission with its days, so the caller can change it and call SaveChangesAsync.
    Task<EventSubmission?> GetForUpdateAsync(int id, CancellationToken cancellationToken = default);

    // The user's submissions with their days, newest first ("My events", 11.4.2). Read-only.
    Task<List<EventSubmission>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);

    // One status for the admin list, oldest first (11.6.1), with days, venue and category. Read-only.
    Task<List<EventSubmission>> GetByStatusAsync(
        EventSubmissionStatus status, int skip, int take, CancellationToken cancellationToken = default);

    // Number of submissions per status; statuses without submissions are missing from the result.
    Task<Dictionary<EventSubmissionStatus, int>> CountByStatusAsync(CancellationToken cancellationToken = default);

    // Persists changes made to submissions loaded through GetForUpdateAsync.
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

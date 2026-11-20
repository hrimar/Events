namespace Events.Services.Media;

// Minimal blob operations needed for images stored by content (GS-REQ-009, 9.1.4, 9.1.6).
public interface IMediaBlobStorage
{
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);

    // Overwrites an existing blob with the same name (same name = same content).
    Task UploadAsync(string blobName, byte[] content, string contentType, CancellationToken cancellationToken = default);

    // Deletes every blob whose name starts with the prefix; returns how many were deleted.
    Task<int> DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}

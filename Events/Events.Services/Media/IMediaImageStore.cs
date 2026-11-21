namespace Events.Services.Media;

// Stores an image we keep locally: original + "large" and "thumb" WebP variants, named by content hash (GS-REQ-009, 9.1.3, 9.1.4, 9.1.6, 9.4).
public interface IMediaImageStore
{
    // requireMinimumSize: true for the Excel import (300x200px, GS-REQ-009, 9.1.3); false for the admin upload.
    Task<MediaStoreResult> StoreAsync(byte[] content, bool requireMinimumSize, CancellationToken cancellationToken = default);

    // Deletes the whole set (thumb, large, original). The caller checks first that no event still uses it.
    Task DeleteAsync(string imageId, CancellationToken cancellationToken = default);
}

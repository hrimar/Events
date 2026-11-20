using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Events.Services.Media;

// IMediaBlobStorage over the "event-images" container (same container and MediaController as GS-REQ-003).
public sealed class AzureMediaBlobStorage : IMediaBlobStorage
{
    private readonly BlobContainerClient _containerClient;

    public AzureMediaBlobStorage(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var response = await _containerClient.GetBlobClient(blobName).ExistsAsync(cancellationToken);
        return response.Value;
    }

    public async Task UploadAsync(string blobName, byte[] content, string contentType, CancellationToken cancellationToken = default)
    {
        // Content type is stored on the blob, so MediaController returns image/webp, image/png etc.
        var options = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } };
        await _containerClient.GetBlobClient(blobName).UploadAsync(new BinaryData(content), options, cancellationToken);
    }

    public async Task<int> DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var deleted = 0;
        await foreach (var blob in _containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, prefix, cancellationToken))
        {
            var response = await _containerClient.DeleteBlobIfExistsAsync(blob.Name, cancellationToken: cancellationToken);
            if (response.Value)
            {
                deleted++;
            }
        }

        return deleted;
    }
}

using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Events.Web.Controllers;

// Streams event/venue images from Azure Blob Storage under our own domain,
// so public URLs never expose the *.blob.core.windows.net host.
[Route("media")]
public class MediaController : Controller
{
    private readonly BlobContainerClient _containerClient;

    public MediaController(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    [HttpGet("{**blobPath}")]
    public async Task<IActionResult> Get(string blobPath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return NotFound();
        }

        var blobClient = _containerClient.GetBlobClient(blobPath);

        var existsResponse = await blobClient.ExistsAsync(cancellationToken);
        if (!existsResponse.Value)
        {
            return NotFound();
        }

        var properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        Response.Headers.CacheControl = "public, max-age=2592000";
        Response.Headers.ETag = properties.Value.ETag.ToString();
        Response.Headers.LastModified = properties.Value.LastModified.ToString("R");

        // All images uploaded through AzureBlobImageService are re-encoded as JPEG.
        var contentType = string.IsNullOrEmpty(properties.Value.ContentType) || properties.Value.ContentType == "application/octet-stream"
            ? "image/jpeg"
            : properties.Value.ContentType;

        var downloadResponse = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

        return File(downloadResponse.Value.Content, contentType);
    }
}

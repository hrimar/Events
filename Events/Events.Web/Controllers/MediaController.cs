using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        Response<BlobDownloadStreamingResult> downloadResponse;
        try
        {
            downloadResponse = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
        {
            return NotFound();
        }

        var details = downloadResponse.Value.Details;

        Response.Headers.CacheControl = "public, max-age=2592000";
        Response.Headers.ETag = details.ETag.ToString();
        Response.Headers.LastModified = details.LastModified.ToString("R");

        // All images uploaded through AzureBlobImageService are re-encoded as JPEG.
        var contentType = string.IsNullOrEmpty(details.ContentType) || details.ContentType == "application/octet-stream"
            ? "image/jpeg"
            : details.ContentType;

        return File(downloadResponse.Value.Content, contentType);
    }
}

using Events.Services.Media;

namespace Events.Web.Models;

// srcset/sizes for event images stored with WebP variants (GS-REQ-009, 9.4; deferred from GS-REQ-007, 7.11.2).
// Only "/media/480|1200/{id}.webp" addresses get a srcset; older uploads and the external images of the ticket
// platforms keep their single address (GS-REQ-009, 9.4, out of scope). Razor drops srcset/sizes when they are null.
public static class EventImageSources
{
    // Standard card in gs-event-grid: 1 column below 768px, 2 up to 1199px, 3 up to 1399px, then 4 (GS-REQ-008, 8.1).
    public const string CardSizes = "(min-width: 1400px) 330px, (min-width: 1200px) 33vw, (min-width: 768px) 50vw, 100vw";

    // Homepage hero slide, about 740px wide from 992px (GS-REQ-007, 7.4).
    public const string HeroSizes = "(min-width: 992px) 740px, 100vw";

    // "Don't miss" card image (150px) and category tile (200px) (GS-REQ-007, 7.7, 7.8).
    public const string FeatureSizes = "150px";
    public const string TileSizes = "200px";

    // Event page image in the 8/12 column from 992px.
    public const string DetailsSizes = "(min-width: 992px) 66vw, 100vw";

    // On a broken image, srcset must go too, otherwise the browser keeps using it instead of the fallback src.
    public const string FallbackOnError =
        "this.onerror=null;this.removeAttribute('srcset');this.src='/images/default_event_image.jpeg'";

    // "/media/480/{id}.webp 480w, /media/1200/{id}.webp 1200w" when imageUrl is either variant; otherwise null.
    // Variants are never upscaled, so for a small original "1200w" overstates the width; the browser then just
    // shows the smaller file scaled, as it does today.
    public static string? BuildSrcSet(string? imageUrl)
    {
        var imageId = MediaImagePaths.TryGetImageId(imageUrl);
        if (imageId == null)
        {
            return null;
        }

        var thumbnail = MediaImagePaths.ToUrl(MediaImagePaths.ThumbnailBlobName(imageId));
        var large = MediaImagePaths.ToUrl(MediaImagePaths.LargeBlobName(imageId));
        return $"{thumbnail} {ImageVariantGenerator.ThumbnailWidth}w, {large} {ImageVariantGenerator.LargeWidth}w";
    }

    // The sizes value only when there is a srcset, so plain images keep their current markup.
    public static string? SizesFor(string? srcSet, string sizes) => srcSet == null ? null : sizes;
}

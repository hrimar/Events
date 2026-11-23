namespace Events.Models.Enums;

// Why an imported image could not be stored (GS-REQ-009, 9.3.3).
// The first six values are the reasons named in the spec; the others make the report more precise.
public enum ImageFetchFailureReason
{
    Timeout = 1,
    NotFound = 2,

    // Signed CDN address (e.g. fbcdn.net "oe" parameter) is past its validity.
    Expired = 3,

    // Content is not JPEG, PNG, WebP or GIF (checked by magic bytes, GS-REQ-009, 9.1.3).
    UnsupportedType = 4,

    // Smaller than 300x200px (GS-REQ-009, 9.1.3).
    TooSmall = 5,

    // Local, internal or reserved address, checked after DNS resolution and on every redirect (GS-REQ-009, 9.1.2).
    ForbiddenAddress = 6,

    // Over 10 MB for an image or 2 MB for a page (GS-REQ-009, 9.1.1, 9.2.1).
    TooLarge = 7,

    // Not https, including a redirect to http (GS-REQ-009, 9.1.1).
    NotHttps = 8,

    // Not an absolute address.
    InvalidUrl = 9,

    // The Ticket URL page has no og:image, twitter:image or JSON-LD Event image (GS-REQ-009, 9.2.2).
    NoImageOnPage = 10,

    // Any other HTTP or network error, including more than 3 redirects.
    HttpError = 11,

    // The image was fetched but could not be decoded or written to storage.
    ProcessingError = 12
}

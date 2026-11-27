using System.ComponentModel.DataAnnotations;

namespace Events.Models.Entities;

// A stored machine translation, keyed by a hash of the source text, so a repeated text (e.g. the same show on
// another date) is translated only once (GS-REQ-010, 10.4).
public class TranslationCacheEntry
{
    // Lowercase hex SHA-256 of the text kind, the model and the normalized source text.
    [Required]
    [MaxLength(64)]
    public string Hash { get; set; } = string.Empty;

    [Required]
    public string TranslatedText { get; set; } = string.Empty;

    // Model that produced the translation, e.g. "claude-haiku-4-5-20251001".
    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

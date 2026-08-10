using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace Events.Web.Infrastructure;

/// <summary>
/// Issues and validates a tamper-resistant "form issued at" token for contact spam checks.
/// </summary>
public sealed class ContactFormTimingProtector
{
    public const string ProtectorPurpose = "ContactForm.IssuedAt.v1";
    public static readonly TimeSpan MinimumFillTime = TimeSpan.FromSeconds(3);
    public static readonly TimeSpan MaximumTokenAge = TimeSpan.FromHours(24);

    private readonly IDataProtector _protector;

    public ContactFormTimingProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
    }

    public string CreateIssuedAtToken()
    {
        return _protector.Protect(DateTimeOffset.UtcNow.UtcTicks.ToString());
    }

    /// <summary>
    /// Returns true when the token is valid and the form was not submitted too quickly.
    /// </summary>
    public bool IsTimingValid(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (!TryUnprotectTicks(token, out var ticks))
        {
            return false;
        }

        if (ticks < DateTimeOffset.MinValue.UtcTicks || ticks > DateTimeOffset.MaxValue.UtcTicks)
        {
            return false;
        }

        var age = DateTimeOffset.UtcNow - new DateTimeOffset(ticks, TimeSpan.Zero);
        return age >= MinimumFillTime && age <= MaximumTokenAge;
    }

    private bool TryUnprotectTicks(string token, out long ticks)
    {
        ticks = 0;

        string ticksText;
        try
        {
            // Unprotect throws CryptographicException for tampered or foreign-key payloads.
            ticksText = _protector.Unprotect(token);
        }
        catch (CryptographicException)
        {
            return false;
        }

        return long.TryParse(ticksText, out ticks);
    }
}

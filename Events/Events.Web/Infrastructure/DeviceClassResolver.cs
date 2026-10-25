namespace Events.Web.Infrastructure;

// Which homepage layout the server renders (GS-REQ-007, general requirement 1).
public enum DeviceClass
{
    // Desktop browsers and tablets (iPad, Android tablets): the new desktop homepage.
    Desktop = 1,

    // Phones: the vertical mobile homepage (day chips and the "This week" tabs).
    Mobile = 2
}

public interface IDeviceClassResolver
{
    DeviceClass Resolve(HttpRequest request);
}

// Detects phones on the server, so desktop-only homepage sections are not rendered for them at all.
// The client hint Sec-CH-UA-Mobile (Chromium browsers send it by default) is authoritative when present;
// otherwise the "Mobi" token in the User-Agent decides (MDN's recommended check). Tablets have neither,
// so they get the desktop layout. Responses that depend on this must send "Vary: Sec-CH-UA-Mobile, User-Agent".
public sealed class DeviceClassResolver : IDeviceClassResolver
{
    public const string MobileClientHintHeader = "Sec-CH-UA-Mobile";
    public const string VaryHeaderValue = "Sec-CH-UA-Mobile, User-Agent";

    private const string MobileUserAgentToken = "Mobi";

    public DeviceClass Resolve(HttpRequest request)
    {
        var mobileHint = request.Headers[MobileClientHintHeader].ToString().Trim();
        if (mobileHint == "?1")
            return DeviceClass.Mobile;

        if (mobileHint == "?0")
            return DeviceClass.Desktop;

        var userAgent = request.Headers.UserAgent.ToString();

        return userAgent.Contains(MobileUserAgentToken, StringComparison.Ordinal) ? DeviceClass.Mobile : DeviceClass.Desktop;
    }
}

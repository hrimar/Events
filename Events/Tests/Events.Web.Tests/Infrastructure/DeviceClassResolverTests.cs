using Events.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Events.Web.Tests.Infrastructure;

public class DeviceClassResolverTests
{
    private const string IPhoneSafari =
        "Mozilla/5.0 (iPhone; CPU iPhone OS 17_5 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Mobile/15E148 Safari/604.1";
    private const string IPadSafari =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Safari/605.1.15";
    private const string AndroidPhoneChrome =
        "Mozilla/5.0 (Linux; Android 14; Pixel 8) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Mobile Safari/537.36";
    private const string AndroidTabletChrome =
        "Mozilla/5.0 (Linux; Android 14; SM-X710) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36";
    private const string AndroidPhoneFirefox = "Mozilla/5.0 (Android 14; Mobile; rv:131.0) Gecko/131.0 Firefox/131.0";
    private const string WindowsChrome =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36";

    private static DeviceClass Resolve(string? userAgent, string? mobileHint = null)
    {
        var context = new DefaultHttpContext();
        if (userAgent != null)
            context.Request.Headers.UserAgent = userAgent;
        if (mobileHint != null)
            context.Request.Headers[DeviceClassResolver.MobileClientHintHeader] = mobileHint;

        return new DeviceClassResolver().Resolve(context.Request);
    }

    [Theory]
    [InlineData(IPhoneSafari)]
    [InlineData(AndroidPhoneChrome)]
    [InlineData(AndroidPhoneFirefox)]
    public void Resolve_PhoneUserAgent_IsMobile(string userAgent)
    {
        Assert.Equal(DeviceClass.Mobile, Resolve(userAgent));
    }

    [Theory]
    [InlineData(IPadSafari)]
    [InlineData(AndroidTabletChrome)]
    [InlineData(WindowsChrome)]
    public void Resolve_TabletOrDesktopUserAgent_IsDesktop(string userAgent)
    {
        Assert.Equal(DeviceClass.Desktop, Resolve(userAgent));
    }

    [Fact]
    public void Resolve_MobileClientHint_WinsOverDesktopUserAgent()
    {
        // Chrome DevTools device mode and "request desktop site" can disagree with the UA string.
        Assert.Equal(DeviceClass.Mobile, Resolve(WindowsChrome, "?1"));
    }

    [Fact]
    public void Resolve_NonMobileClientHint_WinsOverPhoneUserAgent()
    {
        Assert.Equal(DeviceClass.Desktop, Resolve(AndroidPhoneChrome, "?0"));
    }

    [Fact]
    public void Resolve_NoHeaders_IsDesktop()
    {
        Assert.Equal(DeviceClass.Desktop, Resolve(userAgent: null));
    }
}

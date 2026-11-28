using Events.Web.Services.Email;
using Microsoft.Extensions.Localization;

namespace Events.Web.Tests.Services.Email;

public class AccountConfirmationEmailContentTests
{
    // Returns fixed texts, so the test checks the layout and the encoding, not the resource files.
    private sealed class FakeLocalizer(Dictionary<string, string> texts) : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, texts.TryGetValue(name, out var value) ? value : name);

        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            texts.Select(t => new LocalizedString(t.Key, t.Value));
    }

    private static readonly FakeLocalizer Localizer = new(new Dictionary<string, string>
    {
        ["Email_Confirm_Subject"] = "Потвърди имейла си",
        ["Email_Confirm_Greeting"] = "Здравей,",
        ["Email_Confirm_Intro"] = "Отвори линка <по-долу>.",
        ["Email_Confirm_Link"] = "Потвърди",
        ["Email_Confirm_Ignore"] = "Игнорирай писмото."
    });

    [Fact]
    public void Build_UsesLocalizedSubject()
    {
        var content = AccountConfirmationEmailContent.Build(Localizer, "https://www.go-sofia.com/x");

        Assert.Equal("Потвърди имейла си", content.Subject);
    }

    [Fact]
    public void Build_KeepsCyrillicReadable_AndEncodesMarkup()
    {
        var content = AccountConfirmationEmailContent.Build(Localizer, "https://www.go-sofia.com/x");

        Assert.Contains("<p>Здравей,</p>", content.HtmlBody);
        Assert.Contains("Отвори линка &lt;по-долу&gt;.", content.HtmlBody);
        Assert.DoesNotContain("&#x", content.HtmlBody);
    }

    [Fact]
    public void Build_LinkIsEncodedInTheHrefAttribute()
    {
        const string url = "https://www.go-sofia.com/en/Identity/Account/ConfirmEmail?userId=1&code=a\"b&returnUrl=%2Fen%2Fadd-event";

        var content = AccountConfirmationEmailContent.Build(Localizer, url);

        Assert.Contains(
            "<a href=\"https://www.go-sofia.com/en/Identity/Account/ConfirmEmail?userId=1&amp;code=a&quot;b&amp;returnUrl=%2Fen%2Fadd-event\">Потвърди</a>",
            content.HtmlBody);
    }
}

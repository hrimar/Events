using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Localization;

namespace Events.Web.Services.Email;

/// <summary>
/// Text of the e-mail with the account confirmation link, in the language of the current request
/// (GS-REQ-011, 11.1: the organizer form needs a confirmed e-mail, so the message must be readable in BG and EN).
/// </summary>
public static class AccountConfirmationEmailContent
{
    // Keeps Cyrillic readable in the HTML source instead of &#x...; entities (same encoder as the contact form).
    private static readonly HtmlEncoder EmailHtmlEncoder = HtmlEncoder.Create(UnicodeRanges.All);

    public static EmailMessageContent Build(IStringLocalizer localizer, string confirmationUrl)
    {
        var body =
            $"<p>{Encode(localizer["Email_Confirm_Greeting"])}</p>" +
            $"<p>{Encode(localizer["Email_Confirm_Intro"])}</p>" +
            $"<p><a href=\"{EmailHtmlEncoder.Encode(confirmationUrl)}\">{Encode(localizer["Email_Confirm_Link"])}</a></p>" +
            $"<p>{Encode(localizer["Email_Confirm_Ignore"])}</p>";

        return new EmailMessageContent(localizer["Email_Confirm_Subject"], body);
    }

    private static string Encode(LocalizedString text) => EmailHtmlEncoder.Encode(text.Value);
}

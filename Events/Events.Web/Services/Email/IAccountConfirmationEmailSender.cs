using Events.Models.Entities;

namespace Events.Web.Services.Email;

/// <summary>
/// Sends the e-mail with the account confirmation link (GS-REQ-011, 11.1). One place for Register, the resend page,
/// external login and the organizer form, so the text is localized and the return URL is kept everywhere.
/// </summary>
public interface IAccountConfirmationEmailSender
{
    /// <param name="httpContext">The current request; the link keeps its scheme, host and "/en" prefix.</param>
    /// <param name="user">The user whose e-mail is confirmed.</param>
    /// <param name="returnUrl">Local page to offer after the confirmation; ignored when it is not local.</param>
    Task SendAsync(HttpContext httpContext, User user, string? returnUrl);
}

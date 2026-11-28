using System.Text;
using Events.Models.Entities;
using Events.Web.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;

namespace Events.Web.Services.Email;

public sealed class AccountConfirmationEmailSender(
    UserManager<User> userManager,
    LinkGenerator linkGenerator,
    IEmailSender emailSender,
    IStringLocalizer<SharedResources> localizer) : IAccountConfirmationEmailSender
{
    public async Task SendAsync(HttpContext httpContext, User user, string? returnUrl)
    {
        var email = await userManager.GetEmailAsync(user)
            ?? throw new InvalidOperationException("The user has no e-mail address to confirm.");

        var userId = await userManager.GetUserIdAsync(user);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // The request PathBase is used, so a link requested from an English page opens the English confirmation page.
        var confirmationUrl = linkGenerator.GetUriByPage(
            httpContext,
            page: "/Account/ConfirmEmail",
            handler: null,
            values: new { area = "Identity", userId, code, returnUrl = LocalReturnUrl.OrNull(returnUrl) })
            ?? throw new InvalidOperationException("The e-mail confirmation link could not be built.");

        var content = AccountConfirmationEmailContent.Build(localizer, confirmationUrl);
        await emailSender.SendEmailAsync(email, content.Subject, content.HtmlBody);
    }
}

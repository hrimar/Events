namespace Events.Web.Services.Email;

/// <summary>
/// Subject and HTML body of an e-mail, ready for <see cref="Microsoft.AspNetCore.Identity.UI.Services.IEmailSender"/>.
/// </summary>
public sealed record EmailMessageContent(string Subject, string HtmlBody);

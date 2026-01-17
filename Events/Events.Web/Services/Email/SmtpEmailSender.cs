using System.Text;
using Events.Web.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Events.Web.Services.Email;

public class SmtpEmailSender(IOptions<SmtpOptions> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpOptions _emailOptions = options.Value;
    private readonly ILogger<SmtpEmailSender> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(htmlMessage);
        EnsureConfiguration();

        var message = BuildMessage(email, subject, htmlMessage);

        try
        {
            using var client = new SmtpClient();
            var secureSocketOptions = ResolveSecurityOption();

            await client.ConnectAsync(_emailOptions.Host, _emailOptions.Port, secureSocketOptions).ConfigureAwait(false);

            if (!_emailOptions.UseDefaultCredentials && !string.IsNullOrWhiteSpace(_emailOptions.UserName))
            {
                await client.AuthenticateAsync(_emailOptions.UserName, _emailOptions.Password ?? string.Empty).ConfigureAwait(false);
            }

            await client.SendAsync(message).ConfigureAwait(false);
            await client.DisconnectAsync(true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            throw;
        }
    }

    private void EnsureConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_emailOptions.From))
        {
            throw new InvalidOperationException("Smtp:From configuration is required.");
        }

        if (string.IsNullOrWhiteSpace(_emailOptions.Host))
        {
            throw new InvalidOperationException("Smtp:Host configuration is required.");
        }

        if (_emailOptions.Port <= 0)
        {
            throw new InvalidOperationException("Smtp:Port configuration must be a positive number.");
        }
    }

    private MimeMessage BuildMessage(string recipientEmail, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailOptions.DisplayName ?? _emailOptions.From, _emailOptions.From));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlMessage,
            TextBody = StripTags(htmlMessage)
        };

        message.Body = bodyBuilder.ToMessageBody();
        return message;
    }

    private SecureSocketOptions ResolveSecurityOption()
    {
        if (_emailOptions.UseSsl)
        {
            return SecureSocketOptions.SslOnConnect;
        }

        if (_emailOptions.UseStartTls)
        {
            return SecureSocketOptions.StartTls;
        }

        return SecureSocketOptions.Auto;
    }

    private static string StripTags(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(input.Length);
        var insideTag = false;

        foreach (var c in input)
        {
            if (c == '<')
            {
                insideTag = true;
                continue;
            }

            if (c == '>')
            {
                insideTag = false;
                continue;
            }

            if (!insideTag)
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}

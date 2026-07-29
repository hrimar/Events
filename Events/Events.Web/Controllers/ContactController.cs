using System.Text.Encodings.Web;
using Events.Models;
using Events.Services.Interfaces;
using Events.Web.Infrastructure;
using Events.Web.Localization;
using Events.Web.Models;
using Events.Web.Options;
using Events.Web.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Events.Web.Controllers;

public class ContactController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly IStringLocalizer<SharedResources> _localizer;
    private readonly ILogger<ContactController> _logger;
    private readonly ISeoMetaService _seoMetaService;
    private readonly SmtpOptions _smtpOptions;

    public ContactController(
        IEmailSender emailSender,
        IStringLocalizer<SharedResources> localizer,
        ILogger<ContactController> logger,
        ISeoMetaService seoMetaService,
        IOptions<SmtpOptions> smtpOptions)
    {
        _emailSender = emailSender;
        _localizer = localizer;
        _logger = logger;
        _seoMetaService = seoMetaService;
        _smtpOptions = smtpOptions.Value;
    }

    public async Task<IActionResult> Index()
    {
        // Default title (mirrors HomeController.AboutUs) - set before the PageSeoMeta
        // override, not clobbered after, so the page never has empty title/description.
        ViewData["Title"] = _localizer["Contact_Title"];

        var seo = await _seoMetaService.GetByKeyAsync(SeoPageKeys.Contact);
        ViewData.ApplySeoMeta(seo);

        var model = new ContactViewModel
        {
            TopicOptions = ContactTopicLocalizationExtensions.GetSelectListItems(_localizer)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        ViewData["Title"] = _localizer["Contact_Title"];

        // Honeypot hit: pretend success without sending anything or tipping off the bot.
        if (!string.IsNullOrEmpty(model.Website))
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            model.TopicOptions = ContactTopicLocalizationExtensions.GetSelectListItems(_localizer);
            return View(model);
        }

        var topicDisplayName = model.Topic.Localize(_localizer);
        var encodedName = HtmlEncoder.Default.Encode(model.Name);
        var encodedEmail = HtmlEncoder.Default.Encode(model.Email);
        var encodedMessage = HtmlEncoder.Default.Encode(model.Message).Replace("\n", "<br />");

        try
        {
            await _emailSender.SendEmailAsync(_smtpOptions.From,
                $"Ново запитване от {encodedName} — {topicDisplayName}",
                $"<p><strong>Име:</strong> {encodedName}</p>" +
                $"<p><strong>Имейл:</strong> {encodedEmail}</p>" +
                $"<p><strong>Тема:</strong> {topicDisplayName}</p>" +
                $"<p><strong>Съобщение:</strong><br />{encodedMessage}</p>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact form notification to admin for {Email}", model.Email);
            TempData["ErrorMessage"] = _localizer["Contact_ErrorMessage"].Value;
            model.TopicOptions = ContactTopicLocalizationExtensions.GetSelectListItems(_localizer);
            return View(model);
        }

        try
        {
            await _emailSender.SendEmailAsync(model.Email, _localizer["Contact_ConfirmationSubject"],
                $"<p>{HtmlEncoder.Default.Encode(_localizer["Contact_ConfirmationBody"])}</p>");
        }
        catch (Exception ex)
        {
            // The admin already received the message, so this failure shouldn't block the success response.
            _logger.LogError(ex, "Failed to send contact form confirmation email to {Email}", model.Email);
        }

        TempData["SuccessMessage"] = _localizer["Contact_SuccessMessage"].Value;
        return RedirectToAction(nameof(Index));
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Events.Models.Entities;
using Events.Web.Localization;
using Events.Web.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Events.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IAccountConfirmationEmailSender _confirmationEmailSender;
        private readonly IdentityMessages _messages;

        public ResendEmailConfirmationModel(
            UserManager<User> userManager,
            IAccountConfirmationEmailSender confirmationEmailSender,
            IdentityMessages messages)
        {
            _userManager = userManager;
            _confirmationEmailSender = confirmationEmailSender;
            _messages = messages;
        }

        // Page to offer after the confirmation, e.g. the organizer form (GS-REQ-011, 11.1).
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _messages.ResendEmail_Sent);
                return Page();
            }

            await _confirmationEmailSender.SendAsync(HttpContext, user, ReturnUrl);

            ModelState.AddModelError(string.Empty, _messages.ResendEmail_Sent);
            return Page();
        }
    }
}

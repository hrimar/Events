// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events.Models.Entities;
using Events.Web.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Events.Web.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IdentityMessages _messages;

        public ConfirmEmailModel(UserManager<User> userManager, IdentityMessages messages)
        {
            _userManager = userManager;
            _messages = messages;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        // Local page the user came from (e.g. the organizer form); offered after a successful confirmation
        // (GS-REQ-011, 11.1). Null when there is none, it is the home page or it is not local.
        public string ContinueUrl { get; private set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? _messages.ConfirmEmail_Success : _messages.ConfirmEmail_Error;

            if (result.Succeeded && Url.IsLocalUrl(returnUrl) && Url.Content(returnUrl) != Url.Content("~/"))
            {
                ContinueUrl = returnUrl;
            }

            return Page();
        }
    }
}

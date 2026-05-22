using Events.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Events.Web.Localization;

/// <summary>
/// Localized status and error messages for Identity Razor Pages (code-behind).
/// </summary>
public class IdentityMessages(IStringLocalizer<SharedResources> localizer)
{
    public string StatusErrorPrefix => localizer["Msg_StatusErrorPrefix"];

    public string Login_InvalidAttempt => localizer["Msg_Login_InvalidAttempt"];
    public string Login2fa_InvalidCode => localizer["Msg_Login2fa_InvalidCode"];
    public string LoginRecovery_InvalidCode => localizer["Msg_LoginRecovery_InvalidCode"];
    public string ExternalLogin_ErrorFromProvider(string error) => string.Format(localizer["Msg_ExternalLogin_ErrorFromProvider"], error);
    public string ExternalLogin_LoadError => localizer["Msg_ExternalLogin_LoadError"];
    public string ExternalLogin_ConfirmLoadError => localizer["Msg_ExternalLogin_ConfirmLoadError"];

    public string ConfirmEmail_Success => localizer["Msg_ConfirmEmail_Success"];
    public string ConfirmEmail_Error => localizer["Msg_ConfirmEmail_Error"];
    public string ConfirmEmailChange_Error => localizer["Msg_ConfirmEmailChange_Error"];
    public string ConfirmEmailChange_UserNameError => localizer["Msg_ConfirmEmailChange_UserNameError"];
    public string ConfirmEmailChange_Success => localizer["Msg_ConfirmEmailChange_Success"];

    public string ResendEmail_Sent => localizer["Msg_ResendEmail_Sent"];

    public string Manage_ProfileUpdated => localizer["Msg_Manage_ProfileUpdated"];
    public string Manage_PhoneUnexpectedError => localizer["Msg_Manage_PhoneUnexpectedError"];
    public string Manage_PasswordChanged => localizer["Msg_Manage_PasswordChanged"];
    public string Manage_PasswordSet => localizer["Msg_Manage_PasswordSet"];
    public string Manage_EmailUnchanged => localizer["Msg_Manage_EmailUnchanged"];
    public string Manage_EmailChangeSent => localizer["Msg_Manage_EmailChangeSent"];
    public string Manage_EmailVerificationSent => localizer["Msg_Manage_EmailVerificationSent"];
    public string Manage_EmailSendFailed => localizer["Msg_Manage_EmailSendFailed"];
    public string Manage_DeleteIncorrectPassword => localizer["Msg_Manage_DeleteIncorrectPassword"];
    public string Manage_2faDisabled => localizer["Msg_Manage_2faDisabled"];
    public string Manage_2faBrowserForgotten => localizer["Msg_Manage_2faBrowserForgotten"];
    public string Manage_AuthenticatorVerified => localizer["Msg_Manage_AuthenticatorVerified"];
    public string Manage_AuthenticatorInvalidCode => localizer["Msg_Manage_AuthenticatorInvalidCode"];
    public string Manage_AuthenticatorReset => localizer["Msg_Manage_AuthenticatorReset"];
    public string Manage_RecoveryCodesGenerated => localizer["Msg_Manage_RecoveryCodesGenerated"];
    public string Manage_ExternalLoginRemoved => localizer["Msg_Manage_ExternalLoginRemoved"];
    public string Manage_ExternalLoginNotRemoved => localizer["Msg_Manage_ExternalLoginNotRemoved"];
    public string Manage_ExternalLoginAdded => localizer["Msg_Manage_ExternalLoginAdded"];
    public string Manage_ExternalLoginNotAdded => localizer["Msg_Manage_ExternalLoginNotAdded"];
}

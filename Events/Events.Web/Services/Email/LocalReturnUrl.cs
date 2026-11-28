namespace Events.Web.Services.Email;

/// <summary>
/// Keeps a return URL only when it points to this site, the same rule as <c>IUrlHelper.IsLocalUrl</c>: it starts with
/// a single "/" (not "//" or "/\", which browsers treat as another host) or with "~/".
/// </summary>
public static class LocalReturnUrl
{
    public static string? OrNull(string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        if (url[0] == '/')
        {
            return url.Length == 1 || (url[1] != '/' && url[1] != '\\') ? url : null;
        }

        if (url.Length > 1 && url[0] == '~' && url[1] == '/')
        {
            return url.Length == 2 || (url[2] != '/' && url[2] != '\\') ? url : null;
        }

        return null;
    }
}

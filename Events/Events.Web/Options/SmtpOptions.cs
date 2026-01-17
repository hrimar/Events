using System.ComponentModel.DataAnnotations;

namespace Events.Web.Options;

public class SmtpOptions
{
    [Required]
    public string From { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    [Required]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 25;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public bool UseDefaultCredentials { get; set; }

    public bool UseSsl { get; set; }

    public bool UseStartTls { get; set; } = true;
}

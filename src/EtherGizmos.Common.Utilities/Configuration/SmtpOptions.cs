using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.Configuration;

public class SmtpOptions : EmailConnectionOptions
{
    [Required]
    public string Host { get; set; } = null!;

    public int Port { get; set; } = 587;

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public bool UseTls { get; set; } = true;
}

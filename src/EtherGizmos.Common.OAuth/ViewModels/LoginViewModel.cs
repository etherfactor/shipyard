using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Common.ViewModels;

public sealed class LoginViewModel
{
    [Required]
    public string Username { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? ReturnUrl { get; set; }
}

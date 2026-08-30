using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.ViewModels;

public record ChangePasswordViewModel
{
    [Required, DataType(DataType.Password)]
    public string CurrentPassword { get; init; } = null!;

    [Required, DataType(DataType.Password)]
    public string NewPassword { get; init; } = null!;

    [Required, DataType(DataType.Password)]
    public string ConfirmPassword { get; init; } = null!;

    public string? ReturnUrl { get; init; }
}

using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace EtherGizmos.Shipyard.Worker.Configuration;

public class WorkerOptions : IValidateOptions<WorkerOptions>
{
    [Required]
    public string DefaultTimeZone { get; set; } = null!;

    public string? ContainerTimeZone { get; set; }

    public ValidateOptionsResult Validate(
        string? name,
        WorkerOptions options)
    {
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(options.DefaultTimeZone, out _))
            return ValidateOptionsResult.Fail($"The value {options.DefaultTimeZone} is not a valid IANA time zone");

        if (!string.IsNullOrWhiteSpace(options.ContainerTimeZone) && !TimeZoneInfo.TryFindSystemTimeZoneById(options.ContainerTimeZone, out _))
            return ValidateOptionsResult.Fail($"The value {options.ContainerTimeZone} is not a valid IANA time zone");

        return ValidateOptionsResult.Success;
    }
}

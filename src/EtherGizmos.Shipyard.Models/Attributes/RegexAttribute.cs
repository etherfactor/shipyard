using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Models.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class RegexAttribute : ValidationAttribute
{
    public bool AllowEmpty { get; set; } = false;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string stringValue)
            return base.IsValid(value, validationContext);

        try
        {
            _ = new Regex(stringValue);
            return ValidationResult.Success;
        }
        catch (Exception ex)
        {
            return new ValidationResult(ex.Message, [validationContext.MemberName!]);
        }
    }
}

using System.Globalization;
using System.Text;

namespace EtherGizmos.Common.Utilities.Extensions;

public static class StringExtensions
{
    public static string ToFirstUpper(
        this string @this)
    {
        return @this switch
        {
            null => throw new ArgumentNullException(nameof(@this)),
            "" => "",
            _ => string.Concat(@this[0].ToString().ToUpperInvariant(), @this.AsSpan(1))
        };
    }

    public static string SanitizeForTts(
        this string @this)
    {
        var builder = new StringBuilder();

        foreach (var c in @this)
        {
            var category = char.GetUnicodeCategory(c);
            switch (category)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.LetterNumber:
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                //case UnicodeCategory.OpenPunctuation:
                //case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.OtherPunctuation:
                    builder.Append(c);
                    break;
            }
        }

        return builder.ToString();
    }
}

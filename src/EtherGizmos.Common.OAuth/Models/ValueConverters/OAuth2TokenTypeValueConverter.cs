using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2TokenTypeValueConverter : ValueConverter<string?, OAuth2TokenType>
{
    public OAuth2TokenTypeValueConverter()
        : base(
            app => OAuth2TokenTypeConverter.FromString(app),
            db => OAuth2TokenTypeConverter.ToString(db))
    { }
}

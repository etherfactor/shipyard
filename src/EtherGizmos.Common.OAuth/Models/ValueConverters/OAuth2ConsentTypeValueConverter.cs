using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2ConsentTypeValueConverter : ValueConverter<string?, OAuth2ConsentType>
{
    public OAuth2ConsentTypeValueConverter()
        : base(
            app => OAuth2ConsentTypeConverter.FromString(app),
            db => OAuth2ConsentTypeConverter.ToString(db))
    { }
}

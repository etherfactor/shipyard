using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2AuthorizationTypeValueConverter : ValueConverter<string?, OAuth2AuthorizationType>
{
    public OAuth2AuthorizationTypeValueConverter()
        : base(
            app => OAuth2AuthorizationTypeConverter.FromString(app),
            db => OAuth2AuthorizationTypeConverter.ToString(db))
    { }
}

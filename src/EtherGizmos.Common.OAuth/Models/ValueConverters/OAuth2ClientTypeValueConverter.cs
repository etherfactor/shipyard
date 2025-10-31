using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2ClientTypeValueConverter : ValueConverter<string?, OAuth2ClientType>
{
    public OAuth2ClientTypeValueConverter()
        : base(
            app => OAuth2ClientTypeConverter.FromString(app),
            db => OAuth2ClientTypeConverter.ToString(db))
    { }
}

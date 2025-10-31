using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2ApplicationTypeValueConverter : ValueConverter<string?, OAuth2ApplicationType>
{
    public OAuth2ApplicationTypeValueConverter()
        : base(
            app => OAuth2ApplicationTypeConverter.FromString(app),
            db => OAuth2ApplicationTypeConverter.ToString(db))
    { }
}

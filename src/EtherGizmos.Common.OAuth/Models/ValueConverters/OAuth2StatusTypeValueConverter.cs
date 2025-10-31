using EtherGizmos.Common.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class OAuth2StatusTypeValueConverter : ValueConverter<string?, OAuth2StatusType>
{
    public OAuth2StatusTypeValueConverter()
        : base(
            app => OAuth2StatusTypeConverter.FromString(app),
            db => OAuth2StatusTypeConverter.ToString(db))
    { }
}

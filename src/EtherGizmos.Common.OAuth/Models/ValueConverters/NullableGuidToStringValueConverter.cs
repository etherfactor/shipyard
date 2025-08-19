using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EtherGizmos.Common.Models.ValueConverters;

public class NullableGuidToStringValueConverter : ValueConverter<string?, Guid?>
{
    public NullableGuidToStringValueConverter()
        : base(
            app => app != null ? new Guid(app) : null,
            db => db != null ? db.ToString() : null)
    { }
}

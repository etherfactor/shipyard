using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace EtherGizmos.Common;

public static class ShipyardActivitySourcesExtensions
{
    private static readonly ActivitySource _source = new("EtherGizmos.Shipyard");

    extension(ActivitySources)
    {
        public static ActivitySource Shipyard => _source;
    }

    extension(TracerProviderBuilder @this)
    {
        public TracerProviderBuilder AddShipyardInstrumentation()
        {
            @this.AddSource(_source.Name);
            return @this;
        }
    }

    extension(MeterProviderBuilder @this)
    {
        public MeterProviderBuilder AddShipyardInstrumentation()
        {
            return @this;
        }
    }
}

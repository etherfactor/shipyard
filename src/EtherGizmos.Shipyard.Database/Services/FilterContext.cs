using EtherGizmos.Common.Abstractions;

namespace EtherGizmos.Shipyard.Services;

public class FilterContext : IFilterContext
{
    public bool Disabled { get; set; }
}

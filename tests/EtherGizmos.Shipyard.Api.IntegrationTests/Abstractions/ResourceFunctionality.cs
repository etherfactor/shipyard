namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public enum ResourceFunctionality
{
    None = 0,
    Search = 1 << 0,
    Get = 1 << 1,
    Create = 1 << 2,
    Update = 1 << 3,
    Delete = 1 << 4,
    QuerySelect = 1 << 5,
    QueryExpand = 1 << 6,
    QueryFilter = 1 << 7,
    QueryTop = 1 << 8,
    QuerySkip = 1 << 9,
    QueryOrderBy = 1 << 10,
    QueryCount = 1 << 11,
    QueryApply = 1 << 12,
    GroupFiltering = 1 << 16,
}

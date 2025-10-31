namespace EtherGizmos.Common.ViewModels;

public sealed class ConsentScopeViewModel
{
    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string? Description { get; set; }

    public bool IsApproved { get; set; }
}

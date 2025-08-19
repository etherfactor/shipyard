namespace EtherGizmos.Common.ViewModels;

public sealed class ConsentViewModel
{
    public string ApplicationName { get; set; } = "";

    public string ClientId { get; set; } = "";

    public List<ConsentScopeViewModel> Scopes { get; set; } = [];
}

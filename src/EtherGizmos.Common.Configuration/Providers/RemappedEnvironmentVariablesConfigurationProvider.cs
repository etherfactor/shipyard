using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.Text.RegularExpressions;

namespace EtherGizmos.Common.Providers;

internal class RemappedEnvironmentVariablesConfigurationProvider : EnvironmentVariablesConfigurationProvider
{
    private readonly IEnumerable<(Regex Match, string Replacement)> _remaps;

    public RemappedEnvironmentVariablesConfigurationProvider(
        IEnumerable<(Regex Match, string Replacement)> remaps)
    {
        _remaps = remaps;
    }

    public override void Load()
    {
        base.Load();

        var newData = new Dictionary<string, string?>();
        foreach (var datum in Data)
        {
            var key = datum.Key;
            var value = datum.Value;
            newData.Add(key, value);
            foreach (var remap in _remaps)
            {
                var regex = remap.Match;
                var replacement = remap.Replacement;
                key = regex.Replace(key, replacement);
            }

            newData.TryAdd(key, value);
        }

        Data = newData;
    }
}

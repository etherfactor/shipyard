using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EtherGizmos.Shipyard.Worker.Services.Carriers;

internal class RegexClassifier : IRegexClassifier
{
    private readonly IUnitOfWorkFactory _uowFactory;

    private Carrier? _carrier;

    public RegexClassifier(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task<int> ClassifyStatusAsync(
        string slug,
        string description,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();

        var carrierRepo = uow.Repository<Carrier>();

        _carrier ??= await carrierRepo.Data
            .SingleAsync(e => e.Slug == slug, cancellationToken: cancellationToken);

        foreach (var rule in _carrier.Rules.OrderBy(e => e.Priority))
        {
            var regex = new Regex(rule.Pattern);
            if (regex.IsMatch(description))
            {
                return rule.StatusTypeId;
            }
        }

        return StatusTypeId.Unknown;
    }
}

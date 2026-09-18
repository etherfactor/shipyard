using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace EtherGizmos.Shipyard.Events;

public class PackageFailedAttemptRouter : IDomainEventRouter<PackageFailedAttemptEvent>
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public PackageFailedAttemptRouter(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async IAsyncEnumerable<string> FilterScopeAsync(
        PackageFailedAttemptEvent @event,
        IEnumerable<AudienceKey> audiences,
        IEnumerable<string> userIds,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!audiences.Any()) yield break;
        if (!userIds.Any()) yield break;

        using var uow = _uowFactory.Create(new() { AmbientMode = UnitOfWorkAmbientMode.JoinOrCreateAmbient });
        var aclRepo = uow.Repository<AclPackage>();

        var packageIdList = audiences
            .Where(e => "package".Equals(e.Kind, StringComparison.OrdinalIgnoreCase))
            .Select(e => int.TryParse(e.Id, out var i) ? i : default);

        var userIdList = userIds
            .Select(e => Guid.TryParse(e, out var g) ? g : default)
            .Where(e => e != default)
            .ToHashSet();

        var entries = await aclRepo.Data
            .Where(e => userIdList.Contains(e.PrincipalUserId)
                && packageIdList.Contains(e.PackageId)
                && e.PermissionId == PermissionId.Read)
            .ToListAsync(cancellationToken: cancellationToken);

        foreach (var entry in entries)
        {
            yield return entry.PrincipalUserId.ToString();
        }
    }
}

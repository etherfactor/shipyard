using EtherGizmos.Shipyard.Database.Enums;

namespace EtherGizmos.Shipyard.Abstractions;

public interface ICapabilityAuthorizer
{
    void EnsureAuthorized(
        SecurableType securableType,
        int permissionId);
}

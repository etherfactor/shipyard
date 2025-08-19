using EtherGizmos.Common.Abstractions;
using EtherGizmos.Common.Utilities.Abstractions;

namespace EtherGizmos.Shipyard.Models.Database;

public class User : InternalUser<Guid>, IEntity
{
}

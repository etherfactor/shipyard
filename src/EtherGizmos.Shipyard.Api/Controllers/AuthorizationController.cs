using EtherGizmos.Common.Controllers;

namespace EtherGizmos.Shipyard.Controllers;

public class AuthorizationController : AuthorizationControllerBase
{
    public AuthorizationController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}

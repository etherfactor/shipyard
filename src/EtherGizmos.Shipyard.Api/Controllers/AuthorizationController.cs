using EtherGizmos.Common.Controllers;

namespace EtherGizmos.Shipyard.Api.Controllers;

public class AuthorizationController : AuthorizationControllerBase
{
    public AuthorizationController(
        IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }
}

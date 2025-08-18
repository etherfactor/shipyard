using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Token : OpenIddictEntityFrameworkCoreToken<int, OAuth2Application, OAuth2Authorization>
{
}

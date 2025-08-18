using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Application : OpenIddictEntityFrameworkCoreApplication<int, OAuth2Authorization, OAuth2Token>
{
}

using OpenIddict.EntityFrameworkCore.Models;

namespace EtherGizmos.Common.Models;

public class OAuth2Authorization : OpenIddictEntityFrameworkCoreAuthorization<int, OAuth2Application, OAuth2Token>
{
}

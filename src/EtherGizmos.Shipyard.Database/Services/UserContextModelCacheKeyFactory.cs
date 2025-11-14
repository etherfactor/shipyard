using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EtherGizmos.Shipyard.Services;

internal class UserContextModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(
        DbContext context,
        bool designTime)
    {
        var userContext = ((ApplicationContext)context).UserContext;
        return new UserContextModelCacheKey(userContext.UserId);
    }
}

internal record UserContextModelCacheKey(
    Guid? UserId
);

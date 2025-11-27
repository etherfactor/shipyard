namespace EtherGizmos.Common.Abstractions;

public interface IInternalUserStore<TUser>
    : IUserStore<TUser>
{
    Task<bool> ValidatePasswordAsync(
        TUser user,
        string password,
        CancellationToken cancellationToken = default);
}

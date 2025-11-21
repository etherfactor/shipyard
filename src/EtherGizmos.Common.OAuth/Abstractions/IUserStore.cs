namespace EtherGizmos.Common.Abstractions;

public interface IUserStore<TUser>
{
    Task<TUser?> FindBySubjectAsync(
        string subject,
        CancellationToken cancellationToken = default);

    Task<TUser?> FindUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    Task<TUser?> FindUserByEmailAddressAsync(
        string emailAddress,
        CancellationToken cancellationToken = default);

    Task<string> GetSubjectAsync(
        TUser user,
        CancellationToken cancellationToken = default);
}

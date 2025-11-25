namespace EtherGizmos.Common.Abstractions;

public interface IUserLoginHandler<TUser>
{
    Task OnLoginAsync(
        TUser user,
        CancellationToken cancellationToken = default);
}

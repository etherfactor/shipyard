using EtherGizmos.Common.Controllers;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Api.Controllers;

public class InternalAuthenticationController : InternalAuthenticationControllerBase<User>
{
    private readonly IUnitOfWorkFactory _uowFactory;
    private readonly IPasswordHasher<User> _passwordHasher;

    public InternalAuthenticationController(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory,
        IPasswordHasher<User> passwordHasher)
        : base(serviceProvider)
    {
        _uowFactory = uowFactory;
        _passwordHasher = passwordHasher;
    }

    protected override Task<User?> FindUserAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create(useRequestScope: true);
        var repo = uow.Repository<User>();

        var user = repo.Data.SingleOrDefaultAsync(e => e.Username == username, cancellationToken: cancellationToken);
        return user;
    }

    protected override string FindSubject(
        User user)
        => user.Id.ToString();

    protected override Task<bool> ValidatePasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
        => Task.FromResult(
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success);
}

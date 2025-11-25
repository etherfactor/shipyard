using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Identity = Microsoft.AspNetCore.Identity;

namespace EtherGizmos.Shipyard.Services;

internal class UserStore
    : IDisposable,
    IUserStore<User>,
    IInternalUserStore<User>
{
    private readonly IUnitOfWork _uow;
    private readonly Identity.IPasswordHasher<User> _passwordHasher = new Identity.PasswordHasher<User>();

    private bool _disposed;

    public UserStore(
        IServiceProvider serviceProvider,
        IUnitOfWorkFactory uowFactory)
    {
        _uow = uowFactory.AsUnfiltered().Create(serviceProvider);
    }

    public async Task<User?> FindBySubjectAsync(
        string subject,
        CancellationToken cancellationToken = default)
    {
        var userRepo = _uow.Repository<User>();
        if (!Guid.TryParse(subject, out var userId))
            return null;

        return await userRepo.Data.SingleOrDefaultAsync(e => e.Id == userId, cancellationToken: cancellationToken);
    }

    public async Task<User?> FindUserByEmailAddressAsync(
        string emailAddress,
        CancellationToken cancellationToken = default)
    {
        var userRepo = _uow.Repository<User>();
        return await userRepo.Data.SingleOrDefaultAsync(e => e.EmailAddress == emailAddress, cancellationToken: cancellationToken);
    }

    public async Task<User?> FindUserByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var userRepo = _uow.Repository<User>();
        return await userRepo.Data.SingleOrDefaultAsync(e => e.Username == username, cancellationToken: cancellationToken);
    }

    public Task<string> GetSubjectAsync(
        User user,
        CancellationToken cancellationToken = default)
        => Task.FromResult(
            user.Id.ToString());

    public Task<bool> ValidatePasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
        => Task.FromResult(
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) == Identity.PasswordVerificationResult.Success);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _uow.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

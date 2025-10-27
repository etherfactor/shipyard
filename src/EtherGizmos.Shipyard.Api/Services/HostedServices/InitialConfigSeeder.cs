using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Api.Services.HostedServices;

public class InitialConfigSeeder : IHostedService
{
    private readonly IUnitOfWorkFactory _uowFactory;

    public InitialConfigSeeder(
        IUnitOfWorkFactory uowFactory)
    {
        _uowFactory = uowFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        if (!await userRepo.Data.AnyAsync(cancellationToken: cancellationToken))
        {
            var user = new User()
            {
                Username = "admin",
                FullName = "Admin",
                Password = "password",
            };

            userRepo.Create(user);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}

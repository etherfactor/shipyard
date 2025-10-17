using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Api.Services.HostedServices;

public class InitialConfigSeeder : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorkFactory _uowFactory;

    public InitialConfigSeeder(
        IConfiguration configuration,
        IUnitOfWorkFactory uowFactory)
    {
        _configuration = configuration;
        _uowFactory = uowFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();
        var userRepo = uow.Repository<User>();

        var forceCreate = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_FORCE")?.ToLower() == "true";
        var username = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_USER") ?? "admin";
        var password = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_PASSWORD") ?? "password";

        var user = await userRepo.Data.SingleOrDefaultAsync(e => e.Username == username, cancellationToken: cancellationToken);
        if (!await userRepo.Data.AnyAsync(cancellationToken: cancellationToken) || forceCreate)
        {
            if (user is null)
            {
                user ??= new User();
                userRepo.Create(user);
            }

            user.Username = username;
            user.Password = password;

            await uow.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}

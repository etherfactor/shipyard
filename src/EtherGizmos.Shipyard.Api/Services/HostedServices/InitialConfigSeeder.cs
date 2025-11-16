using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
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
                user = new User();
                userRepo.Create(user);
            }

            user.Username = username;
            user.Password = password;
        }

        var groupRepo = uow.Repository<Group>();
        var group = await groupRepo.Data
            .SingleOrDefaultAsync(e => e.SystemId == new Guid("86c51dd9-c62d-49a5-9fa1-87dbd5a95cb5"), cancellationToken: cancellationToken);
        if (group is null)
        {
            group = new Group()
            {
                Name = "Default",
                SystemId = new Guid("86c51dd9-c62d-49a5-9fa1-87dbd5a95cb5"),
            };
            groupRepo.Create(group);
        }

        if (user is not null)
        {
            user.Group ??= group;
        }

        var roleRepo = uow.Repository<Role>();
        var admin = await roleRepo.Data
            .Include(e => e.Principal)
            .ThenInclude(e => e.AclEntries)
            .SingleOrDefaultAsync(e => e.SystemId == new Guid("1706f63d-9bc5-4251-bf61-a50d5c705e08"), cancellationToken: cancellationToken);
        if (admin is null)
        {
            admin = new Role()
            {
                SystemId = new Guid("1706f63d-9bc5-4251-bf61-a50d5c705e08"),
            };
            roleRepo.Create(admin);
        }

        admin.Name = "Administrator";
        admin.Description = "Capable of performing any action within the application.";

        SetPermission(admin.Principal, securableType: SecurableType.Carrier, permissionId: PermissionId.Read, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Carrier, permissionId: PermissionId.Write, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Carrier, permissionId: PermissionId.Delete, grantType: PermissionGrantType.Full);

        SetPermission(admin.Principal, securableType: SecurableType.Package, permissionId: PermissionId.Read, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Package, permissionId: PermissionId.Write, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Package, permissionId: PermissionId.Delete, grantType: PermissionGrantType.Full);

        SetPermission(admin.Principal, securableType: SecurableType.User, permissionId: PermissionId.Read, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.User, permissionId: PermissionId.Write, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.User, permissionId: PermissionId.Delete, grantType: PermissionGrantType.Full);

        SetPermission(admin.Principal, securableType: SecurableType.Role, permissionId: PermissionId.Read, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Role, permissionId: PermissionId.Write, grantType: PermissionGrantType.Full);
        SetPermission(admin.Principal, securableType: SecurableType.Role, permissionId: PermissionId.Delete, grantType: PermissionGrantType.Full);

        var roleUserRepo = uow.Repository<RoleUser>();
        if (!await roleUserRepo.Data.AnyAsync(cancellationToken: cancellationToken))
        {
            var users = await userRepo.Data.ToListAsync(cancellationToken: cancellationToken);
            if (user is not null && !users.Contains(user))
            {
                users.Add(user);
            }

            foreach (var nowAdmin in users)
            {
                nowAdmin.Roles.Add(admin);
            }
        }

        await uow.SaveChangesAsync(cancellationToken);

        await userRepo.Data
            .Where(e => e.GroupId == null)
            .ExecuteUpdateAsync(e => e.SetProperty(e => e.GroupId, _ => group.Id), cancellationToken: cancellationToken);

        var packageRepo = uow.Repository<Package>();
        var missingGroups = await packageRepo.Data
            .Where(e => e.GroupId == null)
            .Include(e => e.CreatedByUser)
            .ToListAsync(cancellationToken: cancellationToken);
        foreach (var package in missingGroups)
        {
            package.GroupId = package.CreatedByUser?.GroupId;
        }

        await uow.SaveChangesAsync(cancellationToken);
    }

    private void SetPermission(
        Principal principal,
        int permissionId,
        PermissionGrantType grantType,
        Guid? securableId = null,
        SecurableType? securableType = null)
    {
        var entry = principal.AclEntries.SingleOrDefault(e =>
            e.PermissionId == permissionId &&
            e.SecurableId == securableId &&
            e.SecurableType == securableType);

        if (entry is null)
        {
            entry = new AclEntry()
            {
                PermissionId = permissionId,
                SecurableId = securableId,
                SecurableType = securableType,
            };

            principal.AclEntries.Add(entry);
        }

        entry.PermissionGrantType = grantType;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}

using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using EtherGizmos.Shipyard.Extensions;
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
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var uow = _uowFactory.Create();

        //Create the initial admin user
        var adminUser = await BootstrapAdminAsync(uow,
            cancellationToken: cancellationToken);

        //Create a default user group
        var defaultGroup = await GetOrCreateGroupAsync(uow,
            systemId: new Guid("86c51dd9-c62d-49a5-9fa1-87dbd5a95cb5"),
            name: "Default",
            description: "The default group.",
            cancellationToken: cancellationToken);

        if (adminUser is not null)
        {
            adminUser.Group ??= defaultGroup;
        }

        //Create system role: System Owner
        var admin = await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("1706f63d-9bc5-4251-bf61-a50d5c705e08"),
            name: "System Owner",
            description: "The System Owner has unrestricted access to all data and configuration in Shipyard. This role can manage carriers, users, roles, and groups across the entire instance and can see every package from every group.",
            permissions:
            [
                //Global R/W/D on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Global R/W/D on packages
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Global R/W/D on users
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Global R/W/D on roles
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Carrier Manager
        await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("a1008dc3-510a-47cf-a6fa-92cf13c9574c"),
            name: "Carrier Manager",
            description: "The Carrier Manager can create, edit, and delete all carriers and tracking integrations. This role is meant for the \"tech person\" who maintains tracking logic without having full system ownership.",
            permissions:
            [
                //Global R/W/D on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Filtered R
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
            ],
            cancellationToken: cancellationToken);

        //Create system role: User Manager
        await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("73b5274e-d972-4669-9a2f-8c1cfe0318dd"),
            name: "User Manager",
            description: "The User Manager handles people, not data. This role can create, edit, and deactivate users across all groups and adjust their group memberships. They can see which roles exist, but cannot change roles, carriers, or any packages; it's focused purely on account and group membership administration.",
            permissions:
            [
                //Global R/W/D on users
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Global R on roles
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Group Owner
        await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("1a72edd6-cc8f-4cb6-b7c6-39364dc73d6f"),
            name: "Group Owner",
            description: "The Group Owner manages everything within a single group. They can add, edit, or remove users in their group and fully manage that group's packages (including soft deletes).",
            permissions:
            [
                //Global R on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),

                //Filtered R/W/D on packages
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Filter),
                
                //Filtered R/W/D on users
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Filter),

                //Global R on roles
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Member
        await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("24f66204-1747-4e44-868c-b9fcd656a772"),
            name: "Member",
            description: "Members are standard users in a group. They can create, edit, and soft-delete packages that belong to their own group, and they can view all carriers configured in the system. They cannot see or manage other groups, users, roles, or carrier configuration.",
            permissions:
            [
                //Global R on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),

                //Filtered R/W/D on packages
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Filter),
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Filter),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Viewer
        await CreateOrUpdateRoleAsync(uow,
            systemId: new Guid("c1f04075-4c1a-493d-a71b-0177c4d8def1"),
            name: "Viewer",
            description: "Viewers can see packages in their own group and view all carriers, but cannot create, edit, or delete anything. This role is ideal for users who just need to check the status of shipments without making any changes.",
            permissions:
            [
                //Global R on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),

                //Filtered R on packages
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
            ],
            cancellationToken: cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);

        //Ensure there is at least one administrator
        await BootstrapUserRolesAsync(uow,
            role: admin,
            cancellationToken: cancellationToken);

        //Ensure all users belong to a group
        await BootstrapUserGroupsAsync(uow,
            group: defaultGroup,
            cancellationToken: cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);

        //Ensure all packages belong to a group
        await BootstrapPackageGroupsAsync(uow,
            cancellationToken: cancellationToken);

        await uow.SaveChangesAsync(cancellationToken);
    }

    private async Task<User?> BootstrapAdminAsync(
        IUnitOfWork uow,
        CancellationToken cancellationToken = default)
    {
        var userRepo = uow.Repository<User>();

        var forceCreate = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_FORCE")?.ToLower() == "true";
        var username = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_USER") ?? "admin";
        var password = _configuration.GetValue<string>("SHIPYARD_BOOTSTRAP_PASSWORD") ?? "password";

        var user = await userRepo.Data.SingleOrDefaultAsync(e => e.Username == username, cancellationToken: cancellationToken);
        if (forceCreate || !await userRepo.Data.AnyAsync(cancellationToken: cancellationToken))
        {
            if (user is null)
            {
                user = new User();
                userRepo.Create(user);
            }

            user.Username = username;
            user.Password = password;
        }

        return user;
    }

    private async Task<Group> GetOrCreateGroupAsync(
        IUnitOfWork uow,
        Guid systemId,
        string name,
        string description,
        CancellationToken cancellationToken = default)
    {
        var groupRepo = uow.Repository<Group>();
        var group = await groupRepo.Data
            .SingleOrDefaultAsync(e => e.SystemId == systemId, cancellationToken: cancellationToken);
        if (group is null)
        {
            group = new Group()
            {
                Name = name,
                Description = description,
                SystemId = systemId,
            };
            groupRepo.Create(group);
        }

        return group;
    }

    private async Task<Role> CreateOrUpdateRoleAsync(
        IUnitOfWork uow,
        Guid systemId,
        string name,
        string description,
        IEnumerable<RolePermission> permissions,
        CancellationToken cancellationToken = default)
    {
        var roleRepo = uow.Repository<Role>();
        var role = await roleRepo.Data
            .Include(e => e.Principal)
            .ThenInclude(e => e.AclEntries)
            .SingleOrDefaultAsync(e => e.SystemId == systemId, cancellationToken: cancellationToken);
        if (role is null)
        {
            role = new Role()
            {
                SystemId = systemId,
            };
            roleRepo.Create(role);
        }

        role.Name = name;
        role.Description = description;

        foreach (var permission in permissions)
        {
            SetPermission(
                role.Principal,
                securableType: permission.SecurableType,
                securableId: permission.SecurableId,
                permissionId: permission.PermissionId,
                grantType: permission.GrantType);
        }

        var desiredKeySet = role.Principal.AclEntries
            .Select(e => (e.PermissionId, e.SecurableId, e.SecurableType))
            .ToHashSet();

        var toRemove = role.Principal.AclEntries
            .Where(e => !desiredKeySet.Contains((e.PermissionId, e.SecurableId, e.SecurableType)))
            .ToList();

        foreach (var missing in toRemove)
        {
            role.Principal.AclEntries.Remove(missing);
        }

        return role;
    }

    private async Task BootstrapUserRolesAsync(
        IUnitOfWork uow,
        Role role,
        CancellationToken cancellationToken = default)
    {
        var roleUserRepo = uow.Repository<RoleUser>();
        if (!await roleUserRepo.Data.AnyAsync(cancellationToken: cancellationToken))
        {
            var userRepo = uow.Repository<User>();
            var users = await userRepo.Data.ToListAsync(cancellationToken: cancellationToken);

            foreach (var nowAdmin in users)
            {
                nowAdmin.Roles.Add(role);
            }
        }
    }

    private async Task BootstrapUserGroupsAsync(
        IUnitOfWork uow,
        Group group,
        CancellationToken cancellationToken = default)
    {
        var userRepo = uow.Repository<User>();

        await userRepo.Data
            .Where(e => e.GroupId == null)
            .ExecuteUpdateAsync(e => e.SetProperty(e => e.GroupId, _ => group.Id), cancellationToken: cancellationToken);
    }

    private async Task BootstrapPackageGroupsAsync(
        IUnitOfWork uow,
        CancellationToken cancellationToken = default)
    {
        var packageRepo = uow.Repository<Package>();
        var missingGroups = await packageRepo.Data
            .Where(e => e.GroupId == null)
            .Include(e => e.CreatedByUser)
            .ToListAsync(cancellationToken: cancellationToken);
        foreach (var package in missingGroups)
        {
            package.GroupId = package.CreatedByUser?.GroupId;
        }
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

    private record RolePermission(
        int PermissionId,
        PermissionGrantType GrantType,
        SecurableType? SecurableType = null,
        Guid? SecurableId = null
    );
}

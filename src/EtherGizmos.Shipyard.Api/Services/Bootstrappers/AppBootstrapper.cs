using EtherGizmos.Common;
using EtherGizmos.Common.Abstractions;
using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace EtherGizmos.Shipyard.Services.Bootstrappers;

internal class AppBootstrapper : IBootstrapper
{
    public int Order => 100;

    private readonly IConfiguration _configuration;
    private readonly IUnitOfWorkFactory _uowFactory;

    public AppBootstrapper(
        IConfiguration configuration,
        IUnitOfWorkFactory uowFactory)
    {
        _configuration = configuration;
        _uowFactory = uowFactory.AsUnfiltered();
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        using var uow = _uowFactory.Create();

        //Create the initial admin user
        var adminUser = await BootstrapAdminAsync(uow,
            cancellationToken: cancellationToken);

        //Create a default user group
        var defaultGroup = await GetOrCreateGroupAsync(uow,
            systemId: AppConstants.Groups.DefaultSystemId,
            name: "Default",
            description: "The default group.",
            cancellationToken: cancellationToken);

        if (adminUser is not null)
        {
            adminUser.Group ??= defaultGroup;
        }

        //Create system role: System Owner
        var admin = await CreateOrUpdateRoleAsync(uow,
            systemId: AppConstants.Roles.SystemOwnerSystemId,
            name: "System Owner",
            description: "The System Owner has unrestricted access to all data and configuration in Shipyard. This role can manage carriers, users, roles, and groups across the entire instance and can see and edit every package from every group.",
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

                //Global R/W/D on groups
                new(SecurableType: SecurableType.Group, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Group, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Group, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Carrier Manager
        await CreateOrUpdateRoleAsync(uow,
            systemId: AppConstants.Roles.CarrierManagerSystemId,
            name: "Carrier Manager",
            description: "The Carrier Manager can create, edit, and delete all carriers and tracking integrations. Grant this role to a user to allow them to connect carriers without necessarily granting them full system access.",
            permissions:
            [
                //Global R/W/D on carriers
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.Carrier, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Filtered R on packages
                new(SecurableType: SecurableType.Package, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
            ],
            cancellationToken: cancellationToken);

        //Create system role: User Manager
        await CreateOrUpdateRoleAsync(uow,
            systemId: AppConstants.Roles.UserManagerSystemId,
            name: "User Manager",
            description: "The User Manager can create, edit, and deactivate users across all groups and adjust their roles and group memberships. Grant this role to a user to allow them to manage other users without necessarily granting them full system access.",
            permissions:
            [
                //Global R/W/D on users
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Write, GrantType: PermissionGrantType.Full),
                new(SecurableType: SecurableType.User, PermissionId: PermissionId.Delete, GrantType: PermissionGrantType.Full),

                //Global R on roles
                new(SecurableType: SecurableType.Role, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),

                //Global R on groups
                new(SecurableType: SecurableType.Group, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Full),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Group Owner
        await CreateOrUpdateRoleAsync(uow,
            systemId: AppConstants.Roles.GroupOwnerSystemId,
            name: "Group Owner",
            description: "The Group Owner manages everything within a single group. They can add, edit, or remove users in their group and fully manage that group's packages.",
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

                //Filtered R on roles
                new(SecurableType: SecurableType.Group, PermissionId: PermissionId.Read, GrantType: PermissionGrantType.Filter),
            ],
            cancellationToken: cancellationToken);

        //Create system role: Member
        await CreateOrUpdateRoleAsync(uow,
            systemId: AppConstants.Roles.MemberSystemId,
            name: "Member",
            description: "Members are standard users in a group. They can create, edit, and delete packages that belong to their own group, and they can view all carriers configured in the system. By default, they cannot see or manage other groups, users, roles, or carrier configuration.",
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
            systemId: AppConstants.Roles.ViewerSystemId,
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

        //Ensure the worker exists
        await CreateOrUpdateSystemUserAsync(uow,
            systemId: AppConstants.Users.WorkerSystemId,
            username: "sys_worker",
            group: defaultGroup,
            roles: [admin],
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
                userRepo.Add(user);
            }

            user.Username = username;
            user.Password = password;
        }

        return user;
    }

    private async Task<User?> CreateOrUpdateSystemUserAsync(
        IUnitOfWork uow,
        Guid systemId,
        string username,
        Group group,
        List<Role> roles,
        CancellationToken cancellationToken = default)
    {
        var userRepo = uow.Repository<User>();

        var user = await userRepo.Data
            .Include(e => e.Group)
            .SingleOrDefaultAsync(e => e.SystemId == systemId, cancellationToken: cancellationToken);
        
        if (user is null)
        {
            user = new();
            userRepo.Add(user);
        }

        user.SystemId = systemId;
        user.Username = username;
        user.Group = group;
        user.IsSystemManaged = true;
        user.IsInteractiveLoginEnabled = false;

        if (user.PasswordHash is null)
        {
            user.Password = "";
        }

        var desiredRoleIds = roles.Select(e => e.Id).ToHashSet();

        var toRemove = user.Roles
            .Where(e => !desiredRoleIds.Contains(e.Id))
            .ToList();

        foreach (var role in toRemove)
        {
            user.Roles.Remove(role);
        }

        var existingRoleIds = user.Roles.Select(e => e.Id).ToHashSet();

        var toAdd = roles
            .Where(e => !existingRoleIds.Contains(e.Id))
            .ToList();

        foreach (var role in toAdd)
        {
            user.Roles.Add(role);
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
            groupRepo.Add(group);
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
            roleRepo.Add(role);
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

        var desiredKeySet = permissions
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

    private record RolePermission(
        int PermissionId,
        PermissionGrantType GrantType,
        SecurableType? SecurableType = null,
        Guid? SecurableId = null
    );
}

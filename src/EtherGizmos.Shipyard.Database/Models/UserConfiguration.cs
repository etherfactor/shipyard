using EtherGizmos.Shipyard.Database;
using EtherGizmos.Shipyard.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EtherGizmos.Shipyard.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .HasColumnName("user_id");

        entity.AuditProperties();

        entity.Property(e => e.SystemId)
            .HasColumnName("system_id");

        entity.Property(e => e.Username)
            .HasColumnName("username");

        entity.Property(e => e.EmailAddress)
            .HasColumnName("email_address");

        entity.Property(e => e.PasswordHash)
            .HasColumnName("password_hash");

        entity.Property(e => e.GivenName)
            .HasColumnName("given_name");

        entity.Property(e => e.FamilyName)
            .HasColumnName("family_name");

        entity.Property(e => e.FullName)
            .HasColumnName("full_name");

        entity.Ignore(e => e.Password);

        entity.Property(e => e.IsSystemManaged)
            .HasColumnName("is_system_managed");

        entity.Property(e => e.IsInteractiveLoginEnabled)
            .HasColumnName("is_interactive_login_enabled");

        entity.Property(e => e.GroupId)
            .HasColumnName("group_id");

        entity.HasOne(e => e.Group)
            .WithMany(e => e.Users)
            .HasForeignKey(e => e.GroupId);

        entity.HasMany(e => e.Roles)
            .WithMany(e => e.Users)
            .UsingEntity<RoleUser>();

        entity.HasMany(e => e.Capabilities)
            .WithOne(e => e.PrincipalUser)
            .HasForeignKey(e => e.PrincipalUserId);

        entity.Property(e => e.PrincipalId)
            .HasColumnName("principal_id");

        entity.HasOne(e => e.Principal);

        entity.Property(e => e.SecurableId)
            .HasColumnName("securable_id");

        entity.HasOne(e => e.Securable);

        entity.HasMany(e => e.AclUserEntries)
            .WithOne(e => e.PrincipalUser)
            .HasForeignKey(e => e.PrincipalUserId);
    }
}

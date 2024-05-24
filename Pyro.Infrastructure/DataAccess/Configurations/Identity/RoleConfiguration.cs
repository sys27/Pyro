// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Identity.Models;
using static Pyro.Domain.Identity.Models.Permission;
using static Pyro.Domain.Identity.Models.Role;

namespace Pyro.Infrastructure.DataAccess.Configurations.Identity;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(x => x.Permissions)
            .WithMany(x => x.Roles)
            .UsingEntity(
                "RolePermission",
                r => r.HasOne(typeof(Permission))
                    .WithMany()
                    .HasForeignKey("PermissionId")
                    .HasPrincipalKey(nameof(Permission.Id))
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(),
                l => l.HasOne(typeof(Role))
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasPrincipalKey(nameof(Role.Id))
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(),
                j =>
                {
                    j.ToTable("RolePermissions");

                    j.HasKey("RoleId", "PermissionId");

                    j.Property<Guid>("RoleId")
                        .HasColumnType("BLOB")
                        .IsRequired();

                    j.Property<Guid>("PermissionId")
                        .HasColumnType("BLOB")
                        .IsRequired();

                    j.HasIndex("PermissionId");

                    j.HasData(
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryView).Id },
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryEdit).Id },
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryManage).Id },
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserView).Id },
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserEdit).Id },
                        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserManage).Id },
                        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(RepositoryView).Id },
                        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(RepositoryEdit).Id },
                        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(UserView).Id },
                        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(UserEdit).Id });
                });

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        builder.HasData(SeedData.Roles);
    }
}
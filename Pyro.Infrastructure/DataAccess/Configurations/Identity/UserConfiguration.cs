// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Identity.Models;
using static Pyro.Domain.Identity.Models.Role;

namespace Pyro.Infrastructure.DataAccess.Configurations.Identity;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public static readonly User[] Users =
    [
        new User
        {
            Id = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966"),
            Email = "pyro@localhost.local",
            Password = [239, 163, 54, 78, 41, 129, 181, 60, 27, 181, 100, 116, 243, 128, 253, 209, 87, 147, 27, 73, 138, 190, 50, 65, 18, 253, 153, 127, 194, 97, 240, 29, 179, 58, 68, 117, 170, 97, 172, 236, 70, 27, 167, 168, 87, 3, 66, 53, 11, 34, 206, 209, 211, 150, 81, 227, 19, 161, 249, 24, 45, 138, 206, 197],
            Salt = [109, 28, 230, 18, 208, 250, 67, 218, 171, 6, 152, 200, 162, 109, 186, 132],
        }
    ];

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .ValueGeneratedNever();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();

        builder.Property(x => x.Salt)
            .IsRequired();

        builder.Property(x => x.IsLocked)
            .HasDefaultValue(false);

        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity(
                "UserRole",
                r => r.HasOne(typeof(Role))
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasPrincipalKey(nameof(Role.Id))
                    .IsRequired(),
                l => l.HasOne(typeof(User))
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasPrincipalKey(nameof(User.Id))
                    .IsRequired(),
                j =>
                {
                    j.ToTable("UserRoles");

                    j.HasKey("UserId", "RoleId");

                    j.Property<Guid>("UserId")
                        .HasColumnType("BLOB")
                        .IsRequired();

                    j.Property<Guid>("RoleId")
                        .HasColumnType("BLOB")
                        .IsRequired();

                    j.HasIndex("RoleId");

                    j.HasData(
                        new { UserId = Users[0].Id, RoleId = SeedData.GetRole(Admin).Id });
                });

        builder.HasMany(x => x.Tokens)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasData(Users);
    }
}
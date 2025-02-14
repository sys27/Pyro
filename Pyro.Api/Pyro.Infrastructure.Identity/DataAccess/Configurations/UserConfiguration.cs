// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.Identity.DataAccess.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Login)
            .IsRequired()
            .HasMaxLength(32)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property<byte[]>("password")
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnType("BLOB")
            .HasColumnName("Password")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.Password);

        builder.Property<byte[]>("salt")
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnType("BLOB")
            .HasColumnName("Salt")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.Salt);

        builder.Property(x => x.PasswordExpiresAt)
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion<DateTimeOffsetToBinaryConverter>();

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
                        .IsRequired();

                    j.Property<Guid>("RoleId")
                        .IsRequired();

                    j.HasIndex("RoleId");

                    j.HasData(SeedData.UserRoles);
                });

        builder.HasMany(x => x.AuthenticationTokens)
            .WithOne(x => x.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AccessTokens)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Login)
            .IsUnique()
            .HasDatabaseName("IX_Users_Login");

        builder.HasData(SeedData.Users);
    }
}
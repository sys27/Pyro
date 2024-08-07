// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.Identity.DataAccess.Configurations;

internal class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
{
    public void Configure(EntityTypeBuilder<AccessToken> builder)
    {
        builder.ToTable("AccessTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property<byte[]>("token")
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnType("BLOB")
            .HasColumnName("Token")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.Token);

        builder.Property<byte[]>("salt")
            .IsRequired()
            .HasMaxLength(16)
            .HasColumnType("BLOB")
            .HasColumnName("Salt")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Ignore(x => x.Salt);

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_AccessTokens_Name");
    }
}
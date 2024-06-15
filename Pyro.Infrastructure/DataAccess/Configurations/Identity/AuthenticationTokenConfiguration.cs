// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.DataAccess.Configurations.Identity;

public class AuthenticationTokenConfiguration : IEntityTypeConfiguration<AuthenticationToken>
{
    public void Configure(EntityTypeBuilder<AuthenticationToken> builder)
    {
        builder.ToTable("AuthenticationTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TokenId)
            .HasColumnType("BLOB")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnType("BLOB")
            .IsRequired();

        builder.HasIndex(x => x.TokenId)
            .IsUnique()
            .HasDatabaseName("IX_AuthenticationTokens_TokenId");
    }
}
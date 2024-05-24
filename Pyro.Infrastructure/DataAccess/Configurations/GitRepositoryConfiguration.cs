// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.GitRepositories;

namespace Pyro.Infrastructure.DataAccess.Configurations;

internal class GitRepositoryConfiguration : IEntityTypeConfiguration<GitRepository>
{
    public void Configure(EntityTypeBuilder<GitRepository> builder)
    {
        builder.ToTable("GitRepositories");

        builder.HasKey(x => x.Id)
            .HasName("PK_GitRepository");

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_GitRepository_Name");
    }
}
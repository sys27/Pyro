// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels", b => b.ExcludeFromMigrations());

        builder.HasKey(x => x.Id)
            .HasName("PK_Label");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Color)
            .IsRequired();

        builder.Property<Guid>("GitRepositoryId")
            .IsRequired();

        builder.Property(x => x.IsDisabled)
            .IsRequired();

        builder.HasIndex("GitRepositoryId", "Name")
            .IsUnique()
            .HasDatabaseName("IX_Label_Name");
    }
}
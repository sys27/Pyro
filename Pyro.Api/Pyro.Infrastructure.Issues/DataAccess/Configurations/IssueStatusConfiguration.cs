// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueStatusConfiguration : IEntityTypeConfiguration<IssueStatus>
{
    public void Configure(EntityTypeBuilder<IssueStatus> builder)
    {
        builder.ToTable("IssueStatuses");

        builder.HasKey(x => x.Id)
            .HasName("PK_IssueStatuses");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Color)
            .IsRequired();

        builder.Property<Guid>("RepositoryId")
            .IsRequired();

        builder.HasOne(x => x.Repository)
            .WithMany(x => x.IssueStatuses)
            .HasForeignKey("RepositoryId")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Property(x => x.IsDisabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(x => x.FromTransitions)
            .WithOne(x => x.From)
            .HasForeignKey("FromId")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(x => x.ToTransitions)
            .WithOne(x => x.To)
            .HasForeignKey("ToId")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_IssueStatuses_Name");
    }
}
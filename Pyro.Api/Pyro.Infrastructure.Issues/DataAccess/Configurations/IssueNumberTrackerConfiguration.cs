// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueNumberTrackerConfiguration : IEntityTypeConfiguration<IssueNumberTrackerConfiguration.IssueNumberTracker>
{
    public void Configure(EntityTypeBuilder<IssueNumberTracker> builder)
    {
        builder.ToTable("IssueNumberTracker");

        builder.HasKey(x => x.RepositoryId)
            .HasName("PK_IssueNumberTracker");

        builder.Property(x => x.RepositoryId)
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasOne(typeof(GitRepository))
            .WithMany()
            .HasForeignKey(nameof(IssueNumberTracker.RepositoryId))
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_IssueNumberTracker_Repository");

        builder.Property(x => x.Number)
            .IsRequired()
            .HasDefaultValue(0);
    }

    internal sealed record IssueNumberTracker(Guid RepositoryId, int Number);
}
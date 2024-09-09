// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueStatusChangeLogConfiguration : IEntityTypeConfiguration<IssueStatusChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueStatusChangeLog> builder)
    {
        builder.HasBaseType<IssueChangeLog>();

        builder.ToTable("IssueStatusChangeLogs");

        builder.Property<Guid?>("OldStatusId")
            .HasColumnName("OldStatusId");

        builder.Property<Guid?>("NewStatusId")
            .HasColumnName("NewStatusId");

        builder.HasOne(x => x.OldStatus)
            .WithMany()
            .HasForeignKey("OldStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.NewStatus)
            .WithMany()
            .HasForeignKey("NewStatusId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
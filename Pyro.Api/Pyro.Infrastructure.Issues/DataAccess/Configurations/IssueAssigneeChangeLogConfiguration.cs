// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueAssigneeChangeLogConfiguration : IEntityTypeConfiguration<IssueAssigneeChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueAssigneeChangeLog> builder)
    {
        builder.HasBaseType<IssueChangeLog>();

        builder.ToTable("IssueAssigneeChangeLogs");

        builder.Property<Guid?>("OldAssigneeId")
            .HasColumnName("OldAssigneeId");

        builder.Property<Guid?>("NewAssigneeId")
            .HasColumnName("NewAssigneeId");

        builder.HasOne(x => x.OldAssignee)
            .WithMany()
            .HasForeignKey("OldAssigneeId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.NewAssignee)
            .WithMany()
            .HasForeignKey("NewAssigneeId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
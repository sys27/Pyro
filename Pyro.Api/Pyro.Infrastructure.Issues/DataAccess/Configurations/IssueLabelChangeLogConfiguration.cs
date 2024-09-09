// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueLabelChangeLogConfiguration : IEntityTypeConfiguration<IssueLabelChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueLabelChangeLog> builder)
    {
        builder.HasBaseType<IssueChangeLog>();

        builder.ToTable("IssueLabelChangeLogs");

        builder.Property<Guid?>("OldLabelId")
            .HasColumnName("OldLabelId");

        builder.Property<Guid?>("NewLabelId")
            .HasColumnName("NewLabelId");

        builder.HasOne(x => x.OldLabel)
            .WithMany()
            .HasForeignKey("OldLabelId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.NewLabel)
            .WithMany()
            .HasForeignKey("NewLabelId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
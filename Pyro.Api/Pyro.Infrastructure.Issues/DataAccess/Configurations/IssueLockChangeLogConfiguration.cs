// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueLockChangeLogConfiguration : IEntityTypeConfiguration<IssueLockChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueLockChangeLog> builder)
    {
        builder.HasBaseType<IssueChangeLog>();

        builder.ToTable("IssueLockChangeLogs");

        builder.Property(x => x.OldValue)
            .HasColumnName("OldValue");

        builder.Property(x => x.NewValue)
            .HasColumnName("NewValue");
    }
}
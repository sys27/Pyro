// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueTitleChangeLogConfiguration : IEntityTypeConfiguration<IssueTitleChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueTitleChangeLog> builder)
    {
        builder.HasBaseType<IssueChangeLog>();

        builder.Property(x => x.OldTitle)
            .HasColumnName("OldValue");

        builder.Property(x => x.NewTitle)
            .HasColumnName("NewValue");
    }
}
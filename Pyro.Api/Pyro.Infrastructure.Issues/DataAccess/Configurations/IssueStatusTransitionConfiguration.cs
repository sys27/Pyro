// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueStatusTransitionConfiguration : IEntityTypeConfiguration<IssueStatusTransition>
{
    public void Configure(EntityTypeBuilder<IssueStatusTransition> builder)
    {
        builder.ToTable("IssueStatusTransitions");

        builder.HasKey(x => x.Id)
            .HasName("PK_IssueStatusTranslations");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property<Guid>("FromId")
            .IsRequired();

        builder.Property<Guid>("ToId")
            .IsRequired();

        builder.HasIndex("FromId", "ToId")
            .IsUnique()
            .HasDatabaseName("IX_IssueStatusTranslations_FromId_ToId");
    }
}
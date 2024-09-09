// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueChangeLogConfiguration : IEntityTypeConfiguration<IssueChangeLog>
{
    public void Configure(EntityTypeBuilder<IssueChangeLog> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(x => x.Id)
            .HasName("PK_IssueChangeLog");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property<Guid>("IssueId")
            .IsRequired();

        builder.HasOne(x => x.Issue)
            .WithMany(x => x.ChangeLogs)
            .HasForeignKey("IssueId")
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion<DateTimeOffsetToBinaryConverter>();

        builder.Property<Guid>("AuthorId")
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
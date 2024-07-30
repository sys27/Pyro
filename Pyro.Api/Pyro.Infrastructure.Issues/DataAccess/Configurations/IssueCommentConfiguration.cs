// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueCommentConfiguration : IEntityTypeConfiguration<IssueComment>
{
    public void Configure(EntityTypeBuilder<IssueComment> builder)
    {
        builder.ToTable("IssueComments");

        builder.HasKey(x => x.Id)
            .HasName("PK_IssueComment");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnType("BLOB");

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property<Guid>("IssueId")
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasOne(x => x.Issue)
            .WithMany(x => x.Comments)
            .HasForeignKey("IssueId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<Guid>("AuthorId")
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion<DateTimeOffsetToBinaryConverter>();
    }
}
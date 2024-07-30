// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issue");

        builder.HasIndex(["RepositoryId", nameof(Issue.IssueNumber)])
            .IsUnique()
            .HasDatabaseName("IX_Issue_RepositoryId_Number");

        builder.HasKey(x => x.Id)
            .HasName("PK_Issue");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnType("BLOB");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IssueNumber)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasValueGenerator<IssueNumberValueGenerator>();

        builder.Property<Guid>("RepositoryId")
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasOne(x => x.Repository)
            .WithMany()
            .HasForeignKey("RepositoryId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<Guid>("AuthorId")
            .IsRequired()
            .HasColumnType("BLOB");

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion<DateTimeOffsetToBinaryConverter>();

        builder.Property<Guid?>("AssigneeId")
            .HasColumnType("BLOB");

        builder.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey("AssigneeId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
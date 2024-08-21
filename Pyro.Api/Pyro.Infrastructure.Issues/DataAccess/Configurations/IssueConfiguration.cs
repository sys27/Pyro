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
        builder.ToTable("Issues");

        builder.HasIndex(["RepositoryId", nameof(Issue.IssueNumber)])
            .IsUnique()
            .HasDatabaseName("IX_Issue_RepositoryId_Number");

        builder.HasKey(x => x.Id)
            .HasName("PK_Issue");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IssueNumber)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .HasValueGenerator<IssueNumberValueGenerator>();

        builder.Property<Guid>("StatusId")
            .IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey("StatusId")
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Issue_Status");

        builder.Property(x => x.RepositoryId)
            .IsRequired();

        builder.HasOne<GitRepository>()
            .WithMany()
            .HasForeignKey(x => x.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<Guid>("AuthorId")
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey("AuthorId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion<DateTimeOffsetToBinaryConverter>();

        builder.Property<Guid?>("AssigneeId");

        builder.HasOne(x => x.Assignee)
            .WithMany()
            .HasForeignKey("AssigneeId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Labels)
            .WithMany()
            .UsingEntity<IssueLabel>(
                r => r.HasOne(x => x.Label)
                    .WithMany()
                    .HasForeignKey(x => x.LabelId)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(),
                l => l.HasOne(x => x.Issue)
                    .WithMany()
                    .HasForeignKey(x => x.IssueId)
                    .HasPrincipalKey(x => x.Id)
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired(),
                j =>
                {
                    j.ToTable("IssueLabels");

                    j.HasKey(x => new { x.IssueId, x.LabelId })
                        .HasName("PK_IssueLabel");

                    j.Property(x => x.IssueId)
                        .IsRequired();

                    j.Property(x => x.LabelId)
                        .IsRequired();
                });
    }
}

public class IssueLabel
{
    public required Guid IssueId { get; set; }
    public required Issue Issue { get; set; }
    public required Guid LabelId { get; set; }
    public required Label Label { get; set; }
}
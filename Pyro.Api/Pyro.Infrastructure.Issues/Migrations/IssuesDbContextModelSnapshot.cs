﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Infrastructure.Issues.DataAccess;

#nullable disable

namespace Pyro.Infrastructure.Issues.Migrations
{
    [DbContext(typeof(IssuesDbContext))]
    partial class IssuesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Pyro.Domain.Issues.GitRepository", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_GitRepository");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_GitRepository_Name");

                    b.ToTable("GitRepositories", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Pyro.Domain.Issues.Issue", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AssigneeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLocked")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<int>("IssueNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("RepositoryId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_Issue");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("StatusId");

                    b.HasIndex("RepositoryId", "IssueNumber")
                        .IsUnique()
                        .HasDatabaseName("IX_Issue_RepositoryId_Number");

                    b.ToTable("Issues", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueChangeLog", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_IssueChangeLog");

                    b.HasIndex("AuthorId");

                    b.HasIndex("IssueId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueComment", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("IssueId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_IssueComment");

                    b.HasIndex("AuthorId");

                    b.HasIndex("IssueId");

                    b.ToTable("IssueComments", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Color")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDisabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RepositoryId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_IssueStatuses");

                    b.HasIndex("RepositoryId", "Name")
                        .IsUnique()
                        .HasDatabaseName("IX_IssueStatuses_Name");

                    b.ToTable("IssueStatuses", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatusTransition", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FromId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ToId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_IssueStatusTranslations");

                    b.HasIndex("ToId");

                    b.HasIndex("FromId", "ToId")
                        .IsUnique()
                        .HasDatabaseName("IX_IssueStatusTranslations_FromId_ToId");

                    b.ToTable("IssueStatusTransitions", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Color")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GitRepositoryId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_Label");

                    b.HasIndex("GitRepositoryId", "Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Label_Name");

                    b.ToTable("Labels", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Pyro.Domain.Issues.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_User");

                    b.ToTable("Users", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("Pyro.Infrastructure.Issues.DataAccess.Configurations.IssueLabel", b =>
                {
                    b.Property<Guid>("IssueId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LabelId")
                        .HasColumnType("TEXT");

                    b.HasKey("IssueId", "LabelId")
                        .HasName("PK_IssueLabel");

                    b.HasIndex("LabelId");

                    b.ToTable("IssueLabels", (string)null);
                });

            modelBuilder.Entity("Pyro.Infrastructure.Issues.DataAccess.Configurations.IssueNumberTrackerConfiguration+IssueNumberTracker", b =>
                {
                    b.Property<Guid>("RepositoryId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.HasKey("RepositoryId")
                        .HasName("PK_IssueNumberTracker");

                    b.ToTable("IssueNumberTracker", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueAssigneeChangeLog", b =>
                {
                    b.HasBaseType("Pyro.Domain.Issues.IssueChangeLog");

                    b.Property<Guid?>("NewAssigneeId")
                        .HasColumnType("TEXT")
                        .HasColumnName("NewAssigneeId");

                    b.Property<Guid?>("OldAssigneeId")
                        .HasColumnType("TEXT")
                        .HasColumnName("OldAssigneeId");

                    b.HasIndex("NewAssigneeId");

                    b.HasIndex("OldAssigneeId");

                    b.ToTable("IssueAssigneeChangeLogs", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueLabelChangeLog", b =>
                {
                    b.HasBaseType("Pyro.Domain.Issues.IssueChangeLog");

                    b.Property<Guid?>("NewLabelId")
                        .HasColumnType("TEXT")
                        .HasColumnName("NewLabelId");

                    b.Property<Guid?>("OldLabelId")
                        .HasColumnType("TEXT")
                        .HasColumnName("OldLabelId");

                    b.HasIndex("NewLabelId");

                    b.HasIndex("OldLabelId");

                    b.ToTable("IssueLabelChangeLogs", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueLockChangeLog", b =>
                {
                    b.HasBaseType("Pyro.Domain.Issues.IssueChangeLog");

                    b.Property<bool>("NewValue")
                        .HasColumnType("INTEGER")
                        .HasColumnName("NewValue");

                    b.Property<bool>("OldValue")
                        .HasColumnType("INTEGER")
                        .HasColumnName("OldValue");

                    b.ToTable("IssueLockChangeLogs", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatusChangeLog", b =>
                {
                    b.HasBaseType("Pyro.Domain.Issues.IssueChangeLog");

                    b.Property<Guid?>("NewStatusId")
                        .HasColumnType("TEXT")
                        .HasColumnName("NewStatusId");

                    b.Property<Guid?>("OldStatusId")
                        .HasColumnType("TEXT")
                        .HasColumnName("OldStatusId");

                    b.HasIndex("NewStatusId");

                    b.HasIndex("OldStatusId");

                    b.ToTable("IssueStatusChangeLogs", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueTitleChangeLog", b =>
                {
                    b.HasBaseType("Pyro.Domain.Issues.IssueChangeLog");

                    b.Property<string>("NewTitle")
                        .HasColumnType("TEXT")
                        .HasColumnName("NewValue");

                    b.Property<string>("OldTitle")
                        .HasColumnType("TEXT")
                        .HasColumnName("OldValue");

                    b.ToTable("IssueTitleChangeLog");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.Issue", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.User", "Assignee")
                        .WithMany()
                        .HasForeignKey("AssigneeId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Pyro.Domain.Issues.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.GitRepository", null)
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.IssueStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired()
                        .HasConstraintName("FK_Issue_Status");

                    b.Navigation("Assignee");

                    b.Navigation("Author");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueChangeLog", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.Issue", "Issue")
                        .WithMany("ChangeLogs")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueComment", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.Issue", "Issue")
                        .WithMany("Comments")
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Issue");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatus", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.GitRepository", "Repository")
                        .WithMany("IssueStatuses")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Repository");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatusTransition", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.IssueStatus", "From")
                        .WithMany("FromTransitions")
                        .HasForeignKey("FromId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.IssueStatus", "To")
                        .WithMany("ToTransitions")
                        .HasForeignKey("ToId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("From");

                    b.Navigation("To");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.Label", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.GitRepository", null)
                        .WithMany("Labels")
                        .HasForeignKey("GitRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pyro.Infrastructure.Issues.DataAccess.Configurations.IssueLabel", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.Issue", "Issue")
                        .WithMany()
                        .HasForeignKey("IssueId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Issues.Label", "Label")
                        .WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Issue");

                    b.Navigation("Label");
                });

            modelBuilder.Entity("Pyro.Infrastructure.Issues.DataAccess.Configurations.IssueNumberTrackerConfiguration+IssueNumberTracker", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.GitRepository", null)
                        .WithMany()
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_IssueNumberTracker_Repository");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueAssigneeChangeLog", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.User", "NewAssignee")
                        .WithMany()
                        .HasForeignKey("NewAssigneeId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Pyro.Domain.Issues.User", "OldAssignee")
                        .WithMany()
                        .HasForeignKey("OldAssigneeId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("NewAssignee");

                    b.Navigation("OldAssignee");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueLabelChangeLog", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.Label", "NewLabel")
                        .WithMany()
                        .HasForeignKey("NewLabelId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Pyro.Domain.Issues.Label", "OldLabel")
                        .WithMany()
                        .HasForeignKey("OldLabelId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("NewLabel");

                    b.Navigation("OldLabel");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatusChangeLog", b =>
                {
                    b.HasOne("Pyro.Domain.Issues.IssueStatus", "NewStatus")
                        .WithMany()
                        .HasForeignKey("NewStatusId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Pyro.Domain.Issues.IssueStatus", "OldStatus")
                        .WithMany()
                        .HasForeignKey("OldStatusId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("NewStatus");

                    b.Navigation("OldStatus");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.GitRepository", b =>
                {
                    b.Navigation("IssueStatuses");

                    b.Navigation("Labels");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.Issue", b =>
                {
                    b.Navigation("ChangeLogs");

                    b.Navigation("Comments");
                });

            modelBuilder.Entity("Pyro.Domain.Issues.IssueStatus", b =>
                {
                    b.Navigation("FromTransitions");

                    b.Navigation("ToTransitions");
                });
#pragma warning restore 612, 618
        }
    }
}

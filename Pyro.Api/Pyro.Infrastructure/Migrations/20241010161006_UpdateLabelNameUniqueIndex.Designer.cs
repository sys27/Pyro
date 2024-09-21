﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Infrastructure.DataAccess;

#nullable disable

namespace Pyro.Infrastructure.Migrations
{
    [DbContext(typeof(PyroDbContext))]
    [Migration("20241010161006_UpdateLabelNameUniqueIndex")]
    partial class UpdateLabelNameUniqueIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Xml")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.GitRepositories.GitRepository", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultBranch")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id")
                        .HasName("PK_GitRepository");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_GitRepository_Name");

                    b.ToTable("GitRepositories", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.GitRepositories.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Color")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("GitRepositoryId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDisabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_Label");

                    b.HasIndex("GitRepositoryId", "Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Label_Name");

                    b.ToTable("Labels", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("Id")
                        .HasName("PK_UserAvatar");

                    b.ToTable("UserAvatars", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_UserProfile");

                    b.ToTable("UserProfiles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                            Email = "pyro@localhost.local",
                            Name = "Pyro"
                        });
                });

            modelBuilder.Entity("Pyro.Infrastructure.Messaging.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Retries")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_OutboxMessages");

                    b.HasIndex("CreatedAt")
                        .HasDatabaseName("IX_OutboxMessages_CreatedAt");

                    b.ToTable("OutboxMessages", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.GitRepositories.Label", b =>
                {
                    b.HasOne("Pyro.Domain.GitRepositories.GitRepository", "GitRepository")
                        .WithMany("Labels")
                        .HasForeignKey("GitRepositoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GitRepository");
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.HasOne("Pyro.Domain.UserProfiles.UserProfile", null)
                        .WithOne("Avatar")
                        .HasForeignKey("Pyro.Domain.UserProfiles.UserAvatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pyro.Domain.GitRepositories.GitRepository", b =>
                {
                    b.Navigation("Labels");
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserProfile", b =>
                {
                    b.Navigation("Avatar");
                });
#pragma warning restore 612, 618
        }
    }
}

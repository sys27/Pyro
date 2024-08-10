﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Infrastructure.DataAccess;

#nullable disable

namespace Pyro.Infrastructure.Migrations
{
    [DbContext(typeof(PyroDbContext))]
    partial class PyroDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

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
                    b.Property<byte[]>("Id")
                        .HasColumnType("BLOB");

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

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasKey("Id")
                        .HasName("PK_UserAvatar");

                    b.ToTable("UserAvatars", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserProfile", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("BLOB");

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
                            Id = new byte[] { 122, 5, 186, 249, 176, 53, 16, 77, 131, 38, 112, 45, 143, 126, 201, 102 },
                            Email = "pyro@localhost.local",
                            Name = "Pyro"
                        });
                });

            modelBuilder.Entity("Pyro.Infrastructure.Messaging.OutboxMessage", b =>
                {
                    b.Property<byte[]>("Id")
                        .HasColumnType("BLOB");

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

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.HasOne("Pyro.Domain.UserProfiles.UserProfile", null)
                        .WithOne("Avatar")
                        .HasForeignKey("Pyro.Domain.UserProfiles.UserAvatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserProfile", b =>
                {
                    b.Navigation("Avatar");
                });
#pragma warning restore 612, 618
        }
    }
}

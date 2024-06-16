﻿// <auto-generated />
using System;
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

            modelBuilder.Entity("Pyro.Domain.GitRepositories.GitRepository", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_GitRepository");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_GitRepository_Name");

                    b.ToTable("GitRepositories", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.AuthenticationToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BLOB");

                    b.Property<DateTimeOffset>("ExpiresAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TokenId")
                        .HasColumnType("BLOB");

                    b.Property<Guid>("UserId")
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("TokenId")
                        .IsUnique()
                        .HasDatabaseName("IX_AuthenticationTokens_TokenId");

                    b.HasIndex("UserId");

                    b.ToTable("AuthenticationTokens", (string)null);
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Permissions_Name");

                    b.ToTable("Permissions", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("f65ad9fd-a259-4598-803a-f85607c7566b"),
                            Name = "repository.view"
                        },
                        new
                        {
                            Id = new Guid("edf38b44-b150-46df-bc79-adaa3c01659f"),
                            Name = "repository.edit"
                        },
                        new
                        {
                            Id = new Guid("a740c470-34ea-46c4-8ca0-dc692e1fb423"),
                            Name = "repository.manage"
                        },
                        new
                        {
                            Id = new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405"),
                            Name = "user.view"
                        },
                        new
                        {
                            Id = new Guid("2c182139-085d-4851-aa3b-ca218ee77e70"),
                            Name = "user.edit"
                        },
                        new
                        {
                            Id = new Guid("e6a86676-0f74-4d00-a9b7-fc84a065d673"),
                            Name = "user.manage"
                        });
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("IX_Roles_Name");

                    b.ToTable("Roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"),
                            Name = "User"
                        });
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BLOB");

                    b.Property<bool>("IsLocked")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("BLOB");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique()
                        .HasDatabaseName("IX_Users_Login");

                    b.ToTable("Users", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                            IsLocked = false,
                            Login = "pyro@localhost.local",
                            Password = new byte[] { 239, 163, 54, 78, 41, 129, 181, 60, 27, 181, 100, 116, 243, 128, 253, 209, 87, 147, 27, 73, 138, 190, 50, 65, 18, 253, 153, 127, 194, 97, 240, 29, 179, 58, 68, 117, 170, 97, 172, 236, 70, 27, 167, 168, 87, 3, 66, 53, 11, 34, 206, 209, 211, 150, 81, 227, 19, 161, 249, 24, 45, 138, 206, 197 },
                            Salt = new byte[] { 109, 28, 230, 18, 208, 250, 67, 218, 171, 6, 152, 200, 162, 109, 186, 132 }
                        });
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BLOB");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasMaxLength(150)
                        .HasColumnType("TEXT");

                    b.HasKey("Id")
                        .HasName("PK_UserProfile");

                    b.ToTable("UserProfiles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                            Name = "Pyro"
                        });
                });

            modelBuilder.Entity("Pyro.Infrastructure.Messaging.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
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

            modelBuilder.Entity("RolePermission", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("BLOB");

                    b.Property<Guid>("PermissionId")
                        .HasColumnType("BLOB");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions", (string)null);

                    b.HasData(
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("f65ad9fd-a259-4598-803a-f85607c7566b")
                        },
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("edf38b44-b150-46df-bc79-adaa3c01659f")
                        },
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("a740c470-34ea-46c4-8ca0-dc692e1fb423")
                        },
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405")
                        },
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("2c182139-085d-4851-aa3b-ca218ee77e70")
                        },
                        new
                        {
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"),
                            PermissionId = new Guid("e6a86676-0f74-4d00-a9b7-fc84a065d673")
                        },
                        new
                        {
                            RoleId = new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"),
                            PermissionId = new Guid("f65ad9fd-a259-4598-803a-f85607c7566b")
                        },
                        new
                        {
                            RoleId = new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"),
                            PermissionId = new Guid("edf38b44-b150-46df-bc79-adaa3c01659f")
                        },
                        new
                        {
                            RoleId = new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"),
                            PermissionId = new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405")
                        },
                        new
                        {
                            RoleId = new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"),
                            PermissionId = new Guid("2c182139-085d-4851-aa3b-ca218ee77e70")
                        });
                });

            modelBuilder.Entity("UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("BLOB");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("BLOB");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                            RoleId = new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d")
                        });
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.AuthenticationToken", b =>
                {
                    b.HasOne("Pyro.Domain.Identity.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.User", b =>
                {
                    b.HasOne("Pyro.Domain.UserProfiles.UserProfile", null)
                        .WithOne()
                        .HasForeignKey("Pyro.Domain.Identity.Models.User", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserAvatar", b =>
                {
                    b.HasOne("Pyro.Domain.UserProfiles.UserProfile", null)
                        .WithOne("Avatar")
                        .HasForeignKey("Pyro.Domain.UserProfiles.UserAvatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RolePermission", b =>
                {
                    b.HasOne("Pyro.Domain.Identity.Models.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Identity.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserRole", b =>
                {
                    b.HasOne("Pyro.Domain.Identity.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pyro.Domain.Identity.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Pyro.Domain.Identity.Models.User", b =>
                {
                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("Pyro.Domain.UserProfiles.UserProfile", b =>
                {
                    b.Navigation("Avatar");
                });
#pragma warning restore 612, 618
        }
    }
}

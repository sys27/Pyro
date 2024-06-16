﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pyro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitRepositories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitRepository", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Retries = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "BLOB", nullable: false),
                    PermissionId = table.Column<Guid>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAvatars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvatar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAvatars_UserProfiles_Id",
                        column: x => x.Id,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<byte[]>(type: "BLOB", maxLength: 64, nullable: false),
                    Salt = table.Column<byte[]>(type: "BLOB", maxLength: 16, nullable: false),
                    IsLocked = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserProfiles_Id",
                        column: x => x.Id,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "BLOB", nullable: false),
                    TokenId = table.Column<Guid>(type: "BLOB", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "BLOB", nullable: false),
                    RoleId = table.Column<Guid>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2c182139-085d-4851-aa3b-ca218ee77e70"), "user.edit" },
                    { new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405"), "user.view" },
                    { new Guid("a740c470-34ea-46c4-8ca0-dc692e1fb423"), "repository.manage" },
                    { new Guid("e6a86676-0f74-4d00-a9b7-fc84a065d673"), "user.manage" },
                    { new Guid("edf38b44-b150-46df-bc79-adaa3c01659f"), "repository.edit" },
                    { new Guid("f65ad9fd-a259-4598-803a-f85607c7566b"), "repository.view" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8"), "User" },
                    { new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"), "Admin" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "Name", "Status" },
                values: new object[] { new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"), "Pyro", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("2c182139-085d-4851-aa3b-ca218ee77e70"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("edf38b44-b150-46df-bc79-adaa3c01659f"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("f65ad9fd-a259-4598-803a-f85607c7566b"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("2c182139-085d-4851-aa3b-ca218ee77e70"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("95fed72d-90b3-4104-891e-a7dae7ea4405"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("a740c470-34ea-46c4-8ca0-dc692e1fb423"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("e6a86676-0f74-4d00-a9b7-fc84a065d673"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("edf38b44-b150-46df-bc79-adaa3c01659f"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("f65ad9fd-a259-4598-803a-f85607c7566b"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Salt" },
                values: new object[] { new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"), "pyro@localhost.local", new byte[] { 239, 163, 54, 78, 41, 129, 181, 60, 27, 181, 100, 116, 243, 128, 253, 209, 87, 147, 27, 73, 138, 190, 50, 65, 18, 253, 153, 127, 194, 97, 240, 29, 179, 58, 68, 117, 170, 97, 172, 236, 70, 27, 167, 168, 87, 3, 66, 53, 11, 34, 206, 209, 211, 150, 81, 227, 19, 161, 249, 24, 45, 138, 206, 197 }, new byte[] { 109, 28, 230, 18, 208, 250, 67, 218, 171, 6, 152, 200, 162, 109, 186, 132 } });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d"), new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966") });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTokens_TokenId",
                table: "AuthenticationTokens",
                column: "TokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTokens_UserId",
                table: "AuthenticationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GitRepository_Name",
                table: "GitRepositories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAt",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationTokens");

            migrationBuilder.DropTable(
                name: "GitRepositories");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserAvatars");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
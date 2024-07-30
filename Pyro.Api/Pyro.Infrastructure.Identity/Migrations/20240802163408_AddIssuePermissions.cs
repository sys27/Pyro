﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pyro.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddIssuePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("327c3c6f-eef4-4f02-b865-cab4d1e550f9"), "issue.manage" },
                    { new Guid("6a351a07-b36e-417e-8cac-33f86f413011"), "issue.view" },
                    { new Guid("773663af-5e24-4468-98c2-607957469e8c"), "issue.edit" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("6a351a07-b36e-417e-8cac-33f86f413011"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("773663af-5e24-4468-98c2-607957469e8c"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") },
                    { new Guid("327c3c6f-eef4-4f02-b865-cab4d1e550f9"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("6a351a07-b36e-417e-8cac-33f86f413011"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") },
                    { new Guid("773663af-5e24-4468-98c2-607957469e8c"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("6a351a07-b36e-417e-8cac-33f86f413011"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("773663af-5e24-4468-98c2-607957469e8c"), new Guid("36b9e20e-9b6b-461b-b129-d6a49fe4f4f8") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("327c3c6f-eef4-4f02-b865-cab4d1e550f9"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("6a351a07-b36e-417e-8cac-33f86f413011"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("773663af-5e24-4468-98c2-607957469e8c"), new Guid("9aa993eb-e3db-4fce-ba9f-b0bb23395b9d") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("327c3c6f-eef4-4f02-b865-cab4d1e550f9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("6a351a07-b36e-417e-8cac-33f86f413011"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("773663af-5e24-4468-98c2-607957469e8c"));
        }
    }
}
﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pyro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLabelNameUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Label_Name",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Labels_GitRepositoryId",
                table: "Labels");

            migrationBuilder.CreateIndex(
                name: "IX_Label_Name",
                table: "Labels",
                columns: new[] { "GitRepositoryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Label_Name",
                table: "Labels");

            migrationBuilder.CreateIndex(
                name: "IX_Label_Name",
                table: "Labels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labels_GitRepositoryId",
                table: "Labels",
                column: "GitRepositoryId");
        }
    }
}
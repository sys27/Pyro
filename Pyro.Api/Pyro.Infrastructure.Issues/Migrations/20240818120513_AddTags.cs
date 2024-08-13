﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pyro.Infrastructure.Issues.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_GitRepositories_RepositoryId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Issue_UserProfiles_AssigneeId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Issue_UserProfiles_AuthorId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueComments_Issue_IssueId",
                table: "IssueComments");

            migrationBuilder.RenameTable(
                name: "Issue",
                newName: "Issues");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_AuthorId",
                table: "Issues",
                newName: "IX_Issues_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_AssigneeId",
                table: "Issues",
                newName: "IX_Issues_AssigneeId");

            migrationBuilder.CreateTable(
                name: "IssueTags",
                columns: table => new
                {
                    IssueId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TagId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueTags", x => new { x.IssueId, x.TagId });
                    table.ForeignKey(
                        name: "FK_IssueTags_Issues_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssueTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueTags_TagId",
                table: "IssueTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueComments_Issues_IssueId",
                table: "IssueComments",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_GitRepositories_RepositoryId",
                table: "Issues",
                column: "RepositoryId",
                principalTable: "GitRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_UserProfiles_AssigneeId",
                table: "Issues",
                column: "AssigneeId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_UserProfiles_AuthorId",
                table: "Issues",
                column: "AuthorId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueComments_Issues_IssueId",
                table: "IssueComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_GitRepositories_RepositoryId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_UserProfiles_AssigneeId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_UserProfiles_AuthorId",
                table: "Issues");

            migrationBuilder.DropTable(
                name: "IssueTags");

            migrationBuilder.RenameTable(
                name: "Issues",
                newName: "Issue");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_AuthorId",
                table: "Issue",
                newName: "IX_Issue_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_AssigneeId",
                table: "Issue",
                newName: "IX_Issue_AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_GitRepositories_RepositoryId",
                table: "Issue",
                column: "RepositoryId",
                principalTable: "GitRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_UserProfiles_AssigneeId",
                table: "Issue",
                column: "AssigneeId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_UserProfiles_AuthorId",
                table: "Issue",
                column: "AuthorId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueComments_Issue_IssueId",
                table: "IssueComments",
                column: "IssueId",
                principalTable: "Issue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
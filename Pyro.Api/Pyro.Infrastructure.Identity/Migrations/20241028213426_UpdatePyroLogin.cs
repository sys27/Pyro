﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pyro.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePyroLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                column: "Login",
                value: "pyro@localhost.local");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f9ba057a-35b0-4d10-8326-702d8f7ec966"),
                column: "Login",
                value: "pyro");
        }
    }
}
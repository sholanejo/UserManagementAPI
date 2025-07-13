using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class up : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("91478e0e-c9af-4bf0-a651-f5e91116fedb"),
                column: "PasswordHash",
                value: "$2b$10$i/BrFfR/2Jzy/VYIG9OTRuqgSr/vLF4OMLG9PNQDhzINAtpehnlQG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("91478e0e-c9af-4bf0-a651-f5e91116fedb"),
                column: "PasswordHash",
                value: "$2a$11$kIToVLDW/v7gPM1gPbqNde9ExQuJvE9EhkTDD0iJDkiKOkF5cTQUW");
        }
    }
}

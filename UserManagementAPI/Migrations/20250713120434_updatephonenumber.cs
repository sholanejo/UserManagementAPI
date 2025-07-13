using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatephonenumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("91478e0e-c9af-4bf0-a651-f5e91116fedb"),
                column: "PhoneNumber",
                value: "+2347031204544");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("91478e0e-c9af-4bf0-a651-f5e91116fedb"),
                column: "PhoneNumber",
                value: "+1234567890");
        }
    }
}

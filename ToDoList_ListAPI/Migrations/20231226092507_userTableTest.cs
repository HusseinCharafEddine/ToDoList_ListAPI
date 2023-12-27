using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList_ListAPI.Migrations
{
    /// <inheritdoc />
    public partial class userTableTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "LocalUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiration",
                table: "LocalUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "LocalUsers");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiration",
                table: "LocalUsers");
        }
    }
}

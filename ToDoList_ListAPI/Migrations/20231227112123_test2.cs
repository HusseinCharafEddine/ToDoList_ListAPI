using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList_ListAPI.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "ListTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "test",
                table: "ListTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

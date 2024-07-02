using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HolyShift.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTestMemberFromEpmloyeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Test",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

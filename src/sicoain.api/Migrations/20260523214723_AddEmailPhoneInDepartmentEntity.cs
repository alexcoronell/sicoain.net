using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sicoain.api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailPhoneInDepartmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Departments");
        }
    }
}

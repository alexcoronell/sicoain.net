using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sicoain.api.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityRoleIdInRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdentityRoleId",
                table: "CustomRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityRoleId",
                table: "CustomRoles");
        }
    }
}

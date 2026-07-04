using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sicoain.api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMainToEmailAndPhoneEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "OccupationalRiskAdministratorPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "OccupationalRiskAdministratorEmails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "HealthPromotionEntityPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "HealthPromotionEntityEmails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "EmployeePhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "EmployeeEmails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "EmployeeContactPhone",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "EmployeeContactEmail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "BusinessPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "BusinessEmails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "BranchPhones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "BranchEmails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "OccupationalRiskAdministratorPhones");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "OccupationalRiskAdministratorEmails");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "HealthPromotionEntityPhones");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "HealthPromotionEntityEmails");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "EmployeePhones");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "EmployeeEmails");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "EmployeeContactPhone");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "EmployeeContactEmail");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "BusinessPhones");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "BusinessEmails");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "BranchPhones");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "BranchEmails");
        }
    }
}

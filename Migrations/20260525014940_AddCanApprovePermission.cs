using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddCanApprovePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanApprove",
                table: "RolePermissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Set CanApprove = true for Administrator role (existing data)
            migrationBuilder.Sql(
                "UPDATE RolePermissions SET CanApprove = 1 WHERE RoleId = '11111111-1111-1111-1111-111111111111'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanApprove",
                table: "RolePermissions");
        }
    }
}

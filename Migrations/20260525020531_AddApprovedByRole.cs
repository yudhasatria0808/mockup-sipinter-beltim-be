using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedByRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByRole",
                table: "WargaNegaraAsings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByRole",
                table: "TenagaKerjaAsings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByRole",
                table: "PotensiKonfliks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByRole",
                table: "PeristiwaKonfliks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByRole",
                table: "KewaspadaanDinis",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByRole",
                table: "WargaNegaraAsings");

            migrationBuilder.DropColumn(
                name: "ApprovedByRole",
                table: "TenagaKerjaAsings");

            migrationBuilder.DropColumn(
                name: "ApprovedByRole",
                table: "PotensiKonfliks");

            migrationBuilder.DropColumn(
                name: "ApprovedByRole",
                table: "PeristiwaKonfliks");

            migrationBuilder.DropColumn(
                name: "ApprovedByRole",
                table: "KewaspadaanDinis");
        }
    }
}

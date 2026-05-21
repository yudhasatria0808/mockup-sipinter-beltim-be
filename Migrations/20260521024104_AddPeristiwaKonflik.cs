using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddPeristiwaKonflik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PeristiwaKonfliks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Periode = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NamaPeristiwa = table.Column<string>(type: "TEXT", nullable: false),
                    SumberSebabKonflik = table.Column<string>(type: "TEXT", nullable: true),
                    LatarBelakangKejadian = table.Column<string>(type: "TEXT", nullable: true),
                    DeskripsiAkibatPeristiwa = table.Column<string>(type: "TEXT", nullable: true),
                    KorbanKritis = table.Column<int>(type: "INTEGER", nullable: false),
                    KorbanLukaLuka = table.Column<int>(type: "INTEGER", nullable: false),
                    KorbanMengungsi = table.Column<int>(type: "INTEGER", nullable: false),
                    KerugianMateril = table.Column<decimal>(type: "TEXT", nullable: false),
                    UpayaPenanganan = table.Column<string>(type: "TEXT", nullable: true),
                    UpayaPemulihan = table.Column<string>(type: "TEXT", nullable: true),
                    Kabupaten = table.Column<string>(type: "TEXT", nullable: false),
                    Kecamatan = table.Column<string>(type: "TEXT", nullable: false),
                    Desa = table.Column<string>(type: "TEXT", nullable: false),
                    AlamatDetail = table.Column<string>(type: "TEXT", nullable: true),
                    TitikKoordinat = table.Column<string>(type: "TEXT", nullable: true),
                    BuktiFoto = table.Column<string>(type: "TEXT", nullable: true),
                    SumberInformasi = table.Column<string>(type: "TEXT", nullable: true),
                    Keterangan = table.Column<string>(type: "TEXT", nullable: true),
                    SaranTindakLanjut = table.Column<string>(type: "TEXT", nullable: true),
                    TingkatRisiko = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CatatanApproval = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedBy = table.Column<string>(type: "TEXT", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeristiwaKonfliks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeristiwaKonfliks");
        }
    }
}

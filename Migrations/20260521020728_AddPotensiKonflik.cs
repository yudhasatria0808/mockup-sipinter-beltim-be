using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddPotensiKonflik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PotensiKonfliks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Periode = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aspek = table.Column<string>(type: "TEXT", nullable: false),
                    Kabupaten = table.Column<string>(type: "TEXT", nullable: false),
                    Kecamatan = table.Column<string>(type: "TEXT", nullable: false),
                    Desa = table.Column<string>(type: "TEXT", nullable: false),
                    AlamatDetail = table.Column<string>(type: "TEXT", nullable: true),
                    TitikKoordinat = table.Column<string>(type: "TEXT", nullable: true),
                    BuktiFoto = table.Column<string>(type: "TEXT", nullable: true),
                    SumberInformasi = table.Column<string>(type: "TEXT", nullable: true),
                    NamaPotensiKonflik = table.Column<string>(type: "TEXT", nullable: false),
                    KemungkinanLevel = table.Column<string>(type: "TEXT", nullable: false),
                    KemungkinanDeskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    SumberSebabPermasalahan = table.Column<string>(type: "TEXT", nullable: true),
                    LatarBelakangMasalah = table.Column<string>(type: "TEXT", nullable: true),
                    DampakLevel = table.Column<string>(type: "TEXT", nullable: false),
                    DampakDeskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    UpayaPenanganan = table.Column<string>(type: "TEXT", nullable: true),
                    KeteranganDetail = table.Column<string>(type: "TEXT", nullable: true),
                    Rekomendasi = table.Column<string>(type: "TEXT", nullable: false),
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
                    table.PrimaryKey("PK_PotensiKonfliks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PotensiKonfliks");
        }
    }
}

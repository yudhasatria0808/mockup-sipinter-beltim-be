using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddWnaAndTka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenagaKerjaAsings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Periode = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NamaTKA = table.Column<string>(type: "TEXT", nullable: false),
                    JenisKelamin = table.Column<string>(type: "TEXT", nullable: false),
                    NamaPerusahaan = table.Column<string>(type: "TEXT", nullable: false),
                    JabatanKeterampilan = table.Column<string>(type: "TEXT", nullable: true),
                    NoTelepon = table.Column<string>(type: "TEXT", nullable: true),
                    Kewarganegaraan = table.Column<string>(type: "TEXT", nullable: false),
                    NoPaspor = table.Column<string>(type: "TEXT", nullable: false),
                    NomorIMTA = table.Column<string>(type: "TEXT", nullable: true),
                    TanggalMulaiIMTA = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TanggalBerakhirIMTA = table.Column<DateTime>(type: "TEXT", nullable: true),
                    JenisIzinTinggal = table.Column<string>(type: "TEXT", nullable: false),
                    Kabupaten = table.Column<string>(type: "TEXT", nullable: false),
                    Kecamatan = table.Column<string>(type: "TEXT", nullable: false),
                    Desa = table.Column<string>(type: "TEXT", nullable: false),
                    AlamatDetail = table.Column<string>(type: "TEXT", nullable: true),
                    TitikKoordinat = table.Column<string>(type: "TEXT", nullable: true),
                    Keterangan = table.Column<string>(type: "TEXT", nullable: true),
                    SumberInformasi = table.Column<string>(type: "TEXT", nullable: true),
                    SaranTindakLanjut = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_TenagaKerjaAsings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WargaNegaraAsings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Periode = table.Column<DateTime>(type: "TEXT", nullable: false),
                    JenisKelamin = table.Column<string>(type: "TEXT", nullable: false),
                    Kewarganegaraan = table.Column<string>(type: "TEXT", nullable: false),
                    NoPaspor = table.Column<string>(type: "TEXT", nullable: false),
                    JenisVisa = table.Column<string>(type: "TEXT", nullable: false),
                    MasaBerlakuVisa = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Pekerjaan = table.Column<string>(type: "TEXT", nullable: true),
                    Sponsor = table.Column<string>(type: "TEXT", nullable: true),
                    Kabupaten = table.Column<string>(type: "TEXT", nullable: false),
                    Kecamatan = table.Column<string>(type: "TEXT", nullable: false),
                    Desa = table.Column<string>(type: "TEXT", nullable: false),
                    AlamatDetail = table.Column<string>(type: "TEXT", nullable: true),
                    TitikKoordinat = table.Column<string>(type: "TEXT", nullable: true),
                    LamaTinggal = table.Column<string>(type: "TEXT", nullable: true),
                    StatusTinggal = table.Column<string>(type: "TEXT", nullable: false),
                    Keterangan = table.Column<string>(type: "TEXT", nullable: true),
                    SumberInformasi = table.Column<string>(type: "TEXT", nullable: true),
                    SaranTindakLanjut = table.Column<string>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_WargaNegaraAsings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenagaKerjaAsings");

            migrationBuilder.DropTable(
                name: "WargaNegaraAsings");
        }
    }
}

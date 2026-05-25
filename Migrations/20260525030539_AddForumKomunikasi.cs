using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddForumKomunikasi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForumPengumumans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Judul = table.Column<string>(type: "TEXT", nullable: false),
                    Isi = table.Column<string>(type: "TEXT", nullable: false),
                    Prioritas = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByRole = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPengumumans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForumTopiks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Judul = table.Column<string>(type: "TEXT", nullable: false),
                    Isi = table.Column<string>(type: "TEXT", nullable: false),
                    Kategori = table.Column<string>(type: "TEXT", nullable: false),
                    Prioritas = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    KewaspadaanDiniId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PeristiwaKonflikId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByRole = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumTopiks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForumArahans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Judul = table.Column<string>(type: "TEXT", nullable: false),
                    Isi = table.Column<string>(type: "TEXT", nullable: false),
                    Prioritas = table.Column<string>(type: "TEXT", nullable: false),
                    InstansiTujuan = table.Column<string>(type: "TEXT", nullable: false),
                    ForumTopikId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByRole = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumArahans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumArahans_ForumTopiks_ForumTopikId",
                        column: x => x.ForumTopikId,
                        principalTable: "ForumTopiks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ForumKomentars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ForumTopikId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Isi = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByRole = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumKomentars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumKomentars_ForumTopiks_ForumTopikId",
                        column: x => x.ForumTopikId,
                        principalTable: "ForumTopiks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumTindakLanjuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ForumArahanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Isi = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TanggalSelesai = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedByRole = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumTindakLanjuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumTindakLanjuts_ForumArahans_ForumArahanId",
                        column: x => x.ForumArahanId,
                        principalTable: "ForumArahans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumArahans_ForumTopikId",
                table: "ForumArahans",
                column: "ForumTopikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumKomentars_ForumTopikId",
                table: "ForumKomentars",
                column: "ForumTopikId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumTindakLanjuts_ForumArahanId",
                table: "ForumTindakLanjuts",
                column: "ForumArahanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumKomentars");

            migrationBuilder.DropTable(
                name: "ForumPengumumans");

            migrationBuilder.DropTable(
                name: "ForumTindakLanjuts");

            migrationBuilder.DropTable(
                name: "ForumArahans");

            migrationBuilder.DropTable(
                name: "ForumTopiks");
        }
    }
}

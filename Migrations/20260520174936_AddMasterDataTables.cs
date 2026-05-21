using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SipintarBeltim.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterDataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aspeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aspeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instansis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instansis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JenisKonfliks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JenisKonfliks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelDampaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Skor = table.Column<int>(type: "INTEGER", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelDampaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelKemungkinans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Skor = table.Column<int>(type: "INTEGER", nullable: false),
                    Deskripsi = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelKemungkinans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelRisikos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    Warna = table.Column<string>(type: "TEXT", nullable: false),
                    SkorMin = table.Column<int>(type: "INTEGER", nullable: false),
                    SkorMax = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelRisikos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wilayahs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipe = table.Column<string>(type: "TEXT", nullable: false),
                    Nama = table.Column<string>(type: "TEXT", nullable: false),
                    KodeBps = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wilayahs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wilayahs_Wilayahs_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Wilayahs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatriksRisikos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    KemungkinanId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DampakId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LevelRisikoId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatriksRisikos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatriksRisikos_LevelDampaks_DampakId",
                        column: x => x.DampakId,
                        principalTable: "LevelDampaks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatriksRisikos_LevelKemungkinans_KemungkinanId",
                        column: x => x.KemungkinanId,
                        principalTable: "LevelKemungkinans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatriksRisikos_LevelRisikos_LevelRisikoId",
                        column: x => x.LevelRisikoId,
                        principalTable: "LevelRisikos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LevelDampaks_Skor",
                table: "LevelDampaks",
                column: "Skor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LevelKemungkinans_Skor",
                table: "LevelKemungkinans",
                column: "Skor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatriksRisikos_DampakId",
                table: "MatriksRisikos",
                column: "DampakId");

            migrationBuilder.CreateIndex(
                name: "IX_MatriksRisikos_KemungkinanId_DampakId",
                table: "MatriksRisikos",
                columns: new[] { "KemungkinanId", "DampakId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatriksRisikos_LevelRisikoId",
                table: "MatriksRisikos",
                column: "LevelRisikoId");

            migrationBuilder.CreateIndex(
                name: "IX_Wilayahs_KodeBps",
                table: "Wilayahs",
                column: "KodeBps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wilayahs_ParentId",
                table: "Wilayahs",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aspeks");

            migrationBuilder.DropTable(
                name: "Instansis");

            migrationBuilder.DropTable(
                name: "JenisKonfliks");

            migrationBuilder.DropTable(
                name: "MatriksRisikos");

            migrationBuilder.DropTable(
                name: "Wilayahs");

            migrationBuilder.DropTable(
                name: "LevelDampaks");

            migrationBuilder.DropTable(
                name: "LevelKemungkinans");

            migrationBuilder.DropTable(
                name: "LevelRisikos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kontokorrent.Migrations
{
    public partial class APIV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Kontokorrent",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Privat",
                table: "Kontokorrent",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BenutzerKontokorrent",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BenutzerId = table.Column<string>(nullable: false),
                    KontokorrentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenutzerKontokorrent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenutzerKontokorrent_Kontokorrent_KontokorrentId",
                        column: x => x.KontokorrentId,
                        principalTable: "Kontokorrent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BenutzerSecret",
                columns: table => new
                {
                    BenutzerId = table.Column<string>(nullable: false),
                    HashedSecret = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenutzerSecret", x => x.BenutzerId);
                });

            migrationBuilder.CreateTable(
                name: "EinladungsCode",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GueltigBis = table.Column<DateTime>(nullable: false),
                    KontokorrentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EinladungsCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EinladungsCode_Kontokorrent_KontokorrentId",
                        column: x => x.KontokorrentId,
                        principalTable: "Kontokorrent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenutzerKontokorrent_KontokorrentId",
                table: "BenutzerKontokorrent",
                column: "KontokorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_EinladungsCode_KontokorrentId",
                table: "EinladungsCode",
                column: "KontokorrentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenutzerKontokorrent");

            migrationBuilder.DropTable(
                name: "BenutzerSecret");

            migrationBuilder.DropTable(
                name: "EinladungsCode");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Kontokorrent");

            migrationBuilder.DropColumn(
                name: "Privat",
                table: "Kontokorrent");
        }
    }
}

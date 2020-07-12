using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kontokorrent.Migrations
{
    public partial class Private : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Kontokorrent",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Secret = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kontokorrent", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    KontokorrentId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.UniqueConstraint("AK_Person_KontokorrentId_Name", x => new { x.KontokorrentId, x.Name });
                    table.ForeignKey(
                        name: "FK_Person_Kontokorrent_KontokorrentId",
                        column: x => x.KontokorrentId,
                        principalTable: "Kontokorrent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bezahlung",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    KontokorrentId = table.Column<string>(nullable: false),
                    BezahlendePersonId = table.Column<string>(nullable: true),
                    Wert = table.Column<double>(nullable: false),
                    Beschreibung = table.Column<string>(nullable: true),
                    Zeitpunkt = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    BearbeitetAm = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bezahlung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bezahlung_Person_BezahlendePersonId",
                        column: x => x.BezahlendePersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bezahlung_Kontokorrent_KontokorrentId",
                        column: x => x.KontokorrentId,
                        principalTable: "Kontokorrent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmfpaengerInBezahlung",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpfaengerId = table.Column<string>(nullable: true),
                    BezahlungId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmfpaengerInBezahlung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmfpaengerInBezahlung_Bezahlung_BezahlungId",
                        column: x => x.BezahlungId,
                        principalTable: "Bezahlung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmfpaengerInBezahlung_Person_EmpfaengerId",
                        column: x => x.EmpfaengerId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenutzerKontokorrent_KontokorrentId",
                table: "BenutzerKontokorrent",
                column: "KontokorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bezahlung_BezahlendePersonId",
                table: "Bezahlung",
                column: "BezahlendePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Bezahlung_KontokorrentId",
                table: "Bezahlung",
                column: "KontokorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_EinladungsCode_KontokorrentId",
                table: "EinladungsCode",
                column: "KontokorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmfpaengerInBezahlung_BezahlungId",
                table: "EmfpaengerInBezahlung",
                column: "BezahlungId");

            migrationBuilder.CreateIndex(
                name: "IX_EmfpaengerInBezahlung_EmpfaengerId",
                table: "EmfpaengerInBezahlung",
                column: "EmpfaengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kontokorrent_Secret",
                table: "Kontokorrent",
                column: "Secret",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenutzerKontokorrent");

            migrationBuilder.DropTable(
                name: "BenutzerSecret");

            migrationBuilder.DropTable(
                name: "EinladungsCode");

            migrationBuilder.DropTable(
                name: "EmfpaengerInBezahlung");

            migrationBuilder.DropTable(
                name: "Bezahlung");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Kontokorrent");
        }
    }
}

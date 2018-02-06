using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Kontokorrent.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kontokorrent",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Secret = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kontokorrent", x => x.Id);
                    table.UniqueConstraint("AK_Kontokorrent_Secret", x => x.Secret);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    KontokorrentId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
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
                    Beschreibung = table.Column<string>(nullable: true),
                    BezahlendePersonId = table.Column<string>(nullable: true),
                    KontokorrentId = table.Column<string>(nullable: false),
                    Wert = table.Column<double>(nullable: false),
                    Zeitpunkt = table.Column<DateTime>(nullable: false)
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
                    BezahlungId = table.Column<string>(nullable: true),
                    EmpfaengerId = table.Column<string>(nullable: true)
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
                name: "IX_Bezahlung_BezahlendePersonId",
                table: "Bezahlung",
                column: "BezahlendePersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Bezahlung_KontokorrentId",
                table: "Bezahlung",
                column: "KontokorrentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmfpaengerInBezahlung_BezahlungId",
                table: "EmfpaengerInBezahlung",
                column: "BezahlungId");

            migrationBuilder.CreateIndex(
                name: "IX_EmfpaengerInBezahlung_EmpfaengerId",
                table: "EmfpaengerInBezahlung",
                column: "EmpfaengerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

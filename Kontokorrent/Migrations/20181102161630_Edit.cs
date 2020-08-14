using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kontokorrent.Migrations
{
    public partial class Edit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BearbeitetAm",
                table: "Bezahlung",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BearbeitetAm",
                table: "Bezahlung");
        }
    }
}

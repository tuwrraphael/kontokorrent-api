using Microsoft.EntityFrameworkCore.Migrations;

namespace Kontokorrent.Migrations
{
    public partial class SoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Bezahlung",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Bezahlung");
        }
    }
}

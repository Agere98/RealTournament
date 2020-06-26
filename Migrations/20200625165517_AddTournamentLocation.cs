using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTournament.Migrations
{
    public partial class AddTournamentLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "Tournament",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "Tournament",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Tournament");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Tournament");
        }
    }
}

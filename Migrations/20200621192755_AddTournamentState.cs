using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTournament.Migrations
{
    public partial class AddTournamentState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ongoing",
                table: "Tournament",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ongoing",
                table: "Tournament");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTournament.Migrations
{
    public partial class FixParticipantIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participant_LicenseNumber",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_Ranking",
                table: "Participant");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_TournamentId_LicenseNumber",
                table: "Participant",
                columns: new[] { "TournamentId", "LicenseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_TournamentId_Ranking",
                table: "Participant",
                columns: new[] { "TournamentId", "Ranking" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participant_TournamentId_LicenseNumber",
                table: "Participant");

            migrationBuilder.DropIndex(
                name: "IX_Participant_TournamentId_Ranking",
                table: "Participant");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_LicenseNumber",
                table: "Participant",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participant_Ranking",
                table: "Participant",
                column: "Ranking",
                unique: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTournament.Migrations
{
    public partial class CreateDataSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tournament",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Discipline = table.Column<string>(nullable: false),
                    Organizer = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(nullable: false),
                    MaxParticipants = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournament", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    TournamentId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    LicenseNumber = table.Column<string>(nullable: false),
                    Ranking = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => new { x.TournamentId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Participant_Tournament_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(nullable: false),
                    Phase = table.Column<int>(nullable: false),
                    FirstPlayerId = table.Column<string>(nullable: false),
                    SecondPlayerId = table.Column<string>(nullable: false),
                    FirstPlayerWin = table.Column<bool>(nullable: true),
                    SecondPlayerWin = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_Tournament_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournament",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Game_Participant_TournamentId_FirstPlayerId",
                        columns: x => new { x.TournamentId, x.FirstPlayerId },
                        principalTable: "Participant",
                        principalColumns: new[] { "TournamentId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Participant_TournamentId_SecondPlayerId",
                        columns: x => new { x.TournamentId, x.SecondPlayerId },
                        principalTable: "Participant",
                        principalColumns: new[] { "TournamentId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentId_FirstPlayerId",
                table: "Game",
                columns: new[] { "TournamentId", "FirstPlayerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentId_SecondPlayerId",
                table: "Game",
                columns: new[] { "TournamentId", "SecondPlayerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Game_TournamentId_Phase_FirstPlayerId_SecondPlayerId",
                table: "Game",
                columns: new[] { "TournamentId", "Phase", "FirstPlayerId", "SecondPlayerId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Tournament_Name_Time",
                table: "Tournament",
                columns: new[] { "Name", "Time" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "Tournament");
        }
    }
}

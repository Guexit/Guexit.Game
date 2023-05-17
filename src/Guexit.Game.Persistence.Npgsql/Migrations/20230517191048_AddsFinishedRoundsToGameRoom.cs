using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddsFinishedRoundsToGameRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards");

            migrationBuilder.AddColumn<Guid>(
                name: "FinishedRoundId",
                table: "SubmittedCards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinishedRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinishedRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinishedRounds_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FinishedRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scores_FinishedRounds_FinishedRoundId",
                        column: x => x.FinishedRoundId,
                        principalTable: "FinishedRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmittedCards_FinishedRoundId",
                table: "SubmittedCards",
                column: "FinishedRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_FinishedRounds_GameRoomId",
                table: "FinishedRounds",
                column: "GameRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_FinishedRoundId",
                table: "Scores",
                column: "FinishedRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards",
                column: "GameRoomId",
                principalTable: "GameRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmittedCards_FinishedRounds_FinishedRoundId",
                table: "SubmittedCards",
                column: "FinishedRoundId",
                principalTable: "FinishedRounds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmittedCards_FinishedRounds_FinishedRoundId",
                table: "SubmittedCards");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "FinishedRounds");

            migrationBuilder.DropIndex(
                name: "IX_SubmittedCards_FinishedRoundId",
                table: "SubmittedCards");

            migrationBuilder.DropColumn(
                name: "FinishedRoundId",
                table: "SubmittedCards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards",
                column: "GameRoomId",
                principalTable: "GameRooms",
                principalColumn: "Id");
        }
    }
}

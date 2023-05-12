using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddsSubmittedCardsByPlayerIdAndRemovesCardIdFromStoryTeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStoryTeller_SelectedCardId",
                table: "GameRooms");

            migrationBuilder.CreateTable(
                name: "SubmittedCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameRoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmittedCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmittedCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmittedCards_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmittedCards_CardId",
                table: "SubmittedCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmittedCards_GameRoomId_PlayerId",
                table: "SubmittedCards",
                columns: new[] { "GameRoomId", "PlayerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmittedCards");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStoryTeller_SelectedCardId",
                table: "GameRooms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

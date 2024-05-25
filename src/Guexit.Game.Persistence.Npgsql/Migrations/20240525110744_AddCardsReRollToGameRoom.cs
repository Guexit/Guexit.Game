using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddCardsReRollToGameRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_PlayerHands_PlayerHandId",
                table: "Cards");

            migrationBuilder.AddColumn<Guid>(
                name: "CardReRollId",
                table: "Cards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CardReRolls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    GameRoomId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardReRolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardReRolls_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardReRollId",
                table: "Cards",
                column: "CardReRollId");

            migrationBuilder.CreateIndex(
                name: "IX_CardReRolls_GameRoomId",
                table: "CardReRolls",
                column: "GameRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_CardReRolls_CardReRollId",
                table: "Cards",
                column: "CardReRollId",
                principalTable: "CardReRolls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards",
                column: "GameRoomId",
                principalTable: "GameRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_PlayerHands_PlayerHandId",
                table: "Cards",
                column: "PlayerHandId",
                principalTable: "PlayerHands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_CardReRolls_CardReRollId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_PlayerHands_PlayerHandId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "CardReRolls");

            migrationBuilder.DropIndex(
                name: "IX_Cards_CardReRollId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CardReRollId",
                table: "Cards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_GameRooms_GameRoomId",
                table: "Cards",
                column: "GameRoomId",
                principalTable: "GameRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_PlayerHands_PlayerHandId",
                table: "Cards",
                column: "PlayerHandId",
                principalTable: "PlayerHands",
                principalColumn: "Id");
        }
    }
}

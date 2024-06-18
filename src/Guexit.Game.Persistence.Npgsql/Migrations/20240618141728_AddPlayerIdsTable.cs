using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerIdsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRoomPlayers",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    GameRoomId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRoomPlayers", x => new { x.GameRoomId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_GameRoomPlayers_GameRooms_GameRoomId",
                        column: x => x.GameRoomId,
                        principalTable: "GameRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                INSERT INTO "GameRoomPlayers" ("GameRoomId", "PlayerId")
                SELECT "Id", TRIM(value) AS PlayerId
                FROM "GameRooms", unnest(string_to_array("PlayerIds", ',')) as value
                WHERE "PlayerIds" IS NOT NULL;
            """);

            migrationBuilder.DropColumn(
                name: "PlayerIds",
                table: "GameRooms");
            
            migrationBuilder.CreateIndex(
                name: "IX_GameRoomPlayers_PlayerId",
                table: "GameRoomPlayers",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRoomPlayers");

            migrationBuilder.AddColumn<string>(
                name: "PlayerIds",
                table: "GameRooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

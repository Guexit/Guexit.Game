using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexInGameRoomCreationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GameRooms_CreatedAt",
                table: "GameRooms",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameRooms_CreatedAt",
                table: "GameRooms");
        }
    }
}

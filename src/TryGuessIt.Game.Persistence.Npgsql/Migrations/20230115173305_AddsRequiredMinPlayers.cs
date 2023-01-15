using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TryGuessIt.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddsRequiredMinPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequiredMinPlayers",
                table: "GameRooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredMinPlayers",
                table: "GameRooms");
        }
    }
}

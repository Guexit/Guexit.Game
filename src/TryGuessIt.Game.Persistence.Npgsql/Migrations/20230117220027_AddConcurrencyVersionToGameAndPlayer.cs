using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TryGuessIt.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyVersionToGameAndPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Players",
                type: "integer",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "GameRooms",
                type: "integer",
                rowVersion: true,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "GameRooms");
        }
    }
}

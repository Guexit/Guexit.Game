using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Sagas.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationPolla2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "DeckAssignmentState",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "DeckAssignmentState");
        }
    }
}

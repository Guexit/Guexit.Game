using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToGameRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "GameRooms",
                type: "text",
                nullable: false,
                defaultValue: "");
            
            SetCreatorPlayerIdForExistingGameRooms(migrationBuilder);
        }

        
        private static void SetCreatorPlayerIdForExistingGameRooms(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE public."GameRooms"
                SET "CreatedBy" = SPLIT_PART("PlayerIds", ',', 1)
                WHERE "PlayerIds" IS NOT NULL AND "PlayerIds" <> '';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GameRooms");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddNickNameToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Players",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            MigrateExistingPlayerNicknames(migrationBuilder);
        }

        private static void MigrateExistingPlayerNicknames(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE public."Players"
                SET "Nickname" = REGEXP_REPLACE(SUBSTRING("Username", 1, POSITION('@' IN "Username") - 1), '[^a-zA-Z0-9]', '', 'g')
                WHERE POSITION('@' IN "Username") > 0;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Players");
        }
    }
}

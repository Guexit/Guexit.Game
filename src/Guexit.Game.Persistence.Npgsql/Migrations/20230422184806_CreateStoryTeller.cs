using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class CreateStoryTeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentStoryTeller_PlayerId",
                table: "GameRooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentStoryTeller_SelectedCardId",
                table: "GameRooms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CurrentStoryTeller_Story",
                table: "GameRooms",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStoryTeller_PlayerId",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CurrentStoryTeller_SelectedCardId",
                table: "GameRooms");

            migrationBuilder.DropColumn(
                name: "CurrentStoryTeller_Story",
                table: "GameRooms");
        }
    }
}

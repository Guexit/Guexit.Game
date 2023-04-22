using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLogicalShardLockingFromImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_LogicalShard_CreatedAt",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "LogicalShard",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CreatedAt",
                table: "Images",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_CreatedAt",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "LogicalShard",
                table: "Images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Images_LogicalShard_CreatedAt",
                table: "Images",
                columns: new[] { "LogicalShard", "CreatedAt" });
        }
    }
}

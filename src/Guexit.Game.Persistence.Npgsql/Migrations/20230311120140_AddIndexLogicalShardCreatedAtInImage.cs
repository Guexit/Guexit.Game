using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexLogicalShardCreatedAtInImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_CreatedAt",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_LogicalShard",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_LogicalShard_CreatedAt",
                table: "Images",
                columns: new[] { "LogicalShard", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_LogicalShard_CreatedAt",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_Images_CreatedAt",
                table: "Images",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Images_LogicalShard",
                table: "Images",
                column: "LogicalShard");
        }
    }
}

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guexit.Game.Persistence.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeMasstransitOutboxPersistance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT FROM information_schema.columns 
                        WHERE table_schema = 'public' 
                          AND table_name = 'OutboxMessage' 
                          AND column_name = 'MessageType'
                    ) THEN
                        ALTER TABLE public."OutboxMessage"
                        ADD COLUMN "MessageType" text NOT NULL DEFAULT '';
                    END IF;
                END
                $$;
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE public."OutboxMessage"
                DROP COLUMN IF EXISTS "MessageType";
            """);
        }
    }
}

using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Mappings;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence;

public sealed class GameDbContext : DbContext
{
    public DbSet<Player> Players { get; set; } = default!;
    public DbSet<GameRoom> GameRooms { get; set; } = default!;
    public DbSet<Image> Images { get; set; } = default!;

    public GameDbContext(DbContextOptions<GameDbContext> contextOptions) : base(contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlayerMappingOverride());
        modelBuilder.ApplyConfiguration(new GameRoomMappingOverride());
        modelBuilder.ApplyConfiguration(new ImageMappingOverride());
        modelBuilder.ApplyConfiguration(new CardMappingOverride());
        modelBuilder.ApplyConfiguration(new PlayerHandMappingOverride());
        modelBuilder.ApplyConfiguration(new SubmittedCardMappingOverride());
        modelBuilder.ApplyConfiguration(new FinishedRoundMappingOverride());
        modelBuilder.ApplyConfiguration(new SubmittedCardSnapshotMappingOverride());
        modelBuilder.ApplyConfiguration(new CardReRollMappingOverride());

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

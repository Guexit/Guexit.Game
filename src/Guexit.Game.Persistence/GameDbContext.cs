using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Persistence.Mappings;

namespace TryGuessIt.Game.Persistence;

public sealed class GameDbContext : DbContext
{
    public DbSet<Player> Players { get; set; } = default!;
	public DbSet<GameRoom> GameRooms { get; set; } = default!;

    public GameDbContext(DbContextOptions<GameDbContext> contextOptions) : base(contextOptions)
	{
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlayerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GameRoomEntityConfiguration());
        
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

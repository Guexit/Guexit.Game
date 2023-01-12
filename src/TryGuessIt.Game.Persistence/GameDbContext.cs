using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;
using TryGuessIt.Game.Persistence.Mappings;

namespace TryGuessIt.Game.Persistence;

public sealed class GameDbContext : DbContext
{
	public DbSet<Player> Players { get; set; }
	public DbSet<GameRoom> GameRooms { get; set; }

	public GameDbContext(DbContextOptions<GameDbContext> contextOptions) : base(contextOptions)
	{

	}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PlayerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GameRoomEntityConfiguration());
    }
}

using Microsoft.EntityFrameworkCore;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Persistence;

public sealed class GameDbContext : DbContext
{
	public DbSet<Player> Players { get; set; }

	public GameDbContext(DbContextOptions<GameDbContext> contextOptions) : base(contextOptions)
	{

	}
}

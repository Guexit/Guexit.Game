using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public bool MigrateOnStartup { get; init; } = false;
}


public sealed class GameDbContextMigrator 
{
    private readonly GameDbContext _context;
    private readonly ILogger<GameDbContextMigrator> _logger;

    public GameDbContextMigrator(GameDbContext dbContext, ILogger<GameDbContextMigrator> logger)
    {
        _context = dbContext;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting migrations...");
        await _context.Database.MigrateAsync(ct);
        _logger.LogInformation("Database migrations applied");
    }
}

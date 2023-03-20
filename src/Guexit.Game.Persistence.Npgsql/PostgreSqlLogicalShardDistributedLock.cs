using System.Data;
using Guexit.Game.Application.CardAssigment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Guexit.Game.Persistence.Npgsql;

public sealed class PostgreSqlLogicalShardDistributedLock : ILogicalShardDistributedLock
{
    private readonly string _connectionString;
    private readonly ILogger<PostgreSqlLogicalShardDistributedLock> _logger;
    private NpgsqlConnection? _connection;

    public PostgreSqlLogicalShardDistributedLock(GameDbContext dbContext, ILogger<PostgreSqlLogicalShardDistributedLock> logger)
    {
        var connectionStringInDbContext = dbContext.Database.GetConnectionString();

        ArgumentException.ThrowIfNullOrEmpty(connectionStringInDbContext);
        
        _connectionString = connectionStringInDbContext;
        _logger = logger;
    }

    public async Task Acquire(int logicalShard, CancellationToken ct = default)
    {
        _connection ??= new NpgsqlConnection(_connectionString);   
        
        if (_connection.State is not ConnectionState.Open) 
            await _connection.OpenAsync(ct);

        await using var command = new NpgsqlCommand("SELECT pg_advisory_lock(@logicalShard)", _connection);
        command.Parameters.AddWithValue("logicalShard", logicalShard);
        await command.ExecuteNonQueryAsync(ct);
        
        _logger.LogInformation("Acquired distributed lock for logical shard {logicalShard}", logicalShard);
    }

    public async Task Release(int logicalShard, CancellationToken ct = default)
    {
        if (_connection is null || _connection.State is not ConnectionState.Open)
            throw new InvalidOperationException("Attempt to release a lock without acquiring it or the connection is closed.");

        await using var command = new NpgsqlCommand("SELECT pg_advisory_unlock(@logicalShard)", _connection);
        command.Parameters.AddWithValue("logicalShard", logicalShard);
        await command.ExecuteNonQueryAsync(ct);
        
        _logger.LogInformation("Released distributed lock for logical shard {logicalShard}", logicalShard);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is null)
            return;

        await _connection.DisposeAsync();
    }
}
using System.Data;
using System.Data.Common;
using Guexit.Game.Application;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Persistence.Interceptors;

public sealed class GameRoomDistributedLockPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IGameRoomCommand
{
    private readonly GameRoomDistributedLock _gameRoomLock;

    public GameRoomDistributedLockPipelineBehavior(GameRoomDistributedLock gameRoomLock)
    {
        _gameRoomLock = gameRoomLock;
    }
    
    public async ValueTask<TResponse> Handle(TCommand command, CancellationToken ct, MessageHandlerDelegate<TCommand, TResponse> next)
    {
        try
        {
            await _gameRoomLock.Acquire(command.GameRoomId, ct);
            
            var result = await next(command, ct);
            
            return result;
        }
        finally
        {
            await _gameRoomLock.Release(command.GameRoomId, ct);
        }
    }
}

public sealed class GameRoomDistributedLock
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

    private bool _wasAcquired;
    private readonly DbConnection _dbConnection;

    public GameRoomDistributedLock(GameDbContext context)
    {
        _dbConnection = context.Database.GetDbConnection();
    }

    public async Task Acquire(GameRoomId gameRoomId, CancellationToken ct = default)
    {
        if (_dbConnection.State is not ConnectionState.Open)
            await _dbConnection.OpenAsync(ct);
        
        await using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT pg_advisory_lock(@LockId);";
        
        var lockIdParameter = command.CreateParameter();
        lockIdParameter.ParameterName = "@LockId";
        lockIdParameter.Value = gameRoomId.GetLongHashCode();

        command.Parameters.Add(lockIdParameter);
        
        var timeoutCancellation = new CancellationTokenSource(DefaultTimeout);
        var joinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellation.Token, ct);

        await command.ExecuteNonQueryAsync(joinedCancellationToken.Token);
        
        _wasAcquired = true;
    }

    public async ValueTask Release(GameRoomId gameRoomId, CancellationToken ct = default)
    {
        if (!_wasAcquired)
            return;
        
        await using var command = _dbConnection.CreateCommand();
        command.CommandText = "SELECT pg_advisory_unlock(@LockId);";
        
        var lockIdParameter = command.CreateParameter();
        lockIdParameter.ParameterName = "@LockId";
        lockIdParameter.Value = gameRoomId.GetLongHashCode();

        command.Parameters.Add(lockIdParameter);
        
        await command.ExecuteNonQueryAsync(ct);
    }
}
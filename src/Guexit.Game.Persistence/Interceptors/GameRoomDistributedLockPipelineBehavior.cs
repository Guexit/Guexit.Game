using Guexit.Game.Application;
using Mediator;

namespace Guexit.Game.Persistence.Interceptors;

public sealed class GameRoomDistributedLockPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IGameRoomCommand<TResponse>
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
using Guexit.Game.Application;
using Mediator;

namespace Guexit.Game.Persistence.Interceptors;

public sealed class GameRoomOptimisticConcurrencyCheckPipelineBehaviour<TGameRoomCommand, TResponse> : IPipelineBehavior<TGameRoomCommand, TResponse>
    where TGameRoomCommand : IGameRoomCommand<TResponse>
{
    private readonly GameRoomOptimisticConcurrencyCheckEnforcer _gameRoomConcurrencyCheckEnforcer;

    public GameRoomOptimisticConcurrencyCheckPipelineBehaviour(GameRoomOptimisticConcurrencyCheckEnforcer gameRoomConcurrencyCheckEnforcer)
    {
        _gameRoomConcurrencyCheckEnforcer = gameRoomConcurrencyCheckEnforcer;
    }
    
    public async ValueTask<TResponse> Handle(TGameRoomCommand command, CancellationToken ct, MessageHandlerDelegate<TGameRoomCommand, TResponse> next)
    {
        var response = await next(command, ct);
        
        _gameRoomConcurrencyCheckEnforcer.ForceConcurrencyTokenCheckFor(command.GameRoomId);
        
        return response;
    }
}
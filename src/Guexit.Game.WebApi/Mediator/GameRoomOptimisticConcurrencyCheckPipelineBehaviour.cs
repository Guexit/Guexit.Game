using Guexit.Game.Application;
using Guexit.Game.Persistence;
using Mediator;

namespace Guexit.Game.WebApi.Mediator;

public sealed class GameRoomOptimisticConcurrencyCheckPipelineBehaviour<TGameRoomCommand, TResponse> : IPipelineBehavior<TGameRoomCommand, TResponse>
    where TGameRoomCommand : IGameRoomCommand
{
    private readonly GameRoomOptimisticConcurrencyCheckEnforcer _gameRoomConcurrencyCheckEnforcer;

    public GameRoomOptimisticConcurrencyCheckPipelineBehaviour(GameRoomOptimisticConcurrencyCheckEnforcer gameRoomConcurrencyCheckEnforcer)
    {
        _gameRoomConcurrencyCheckEnforcer = gameRoomConcurrencyCheckEnforcer;
    }
    
    public async ValueTask<TResponse> Handle(TGameRoomCommand command, CancellationToken ct, MessageHandlerDelegate<TGameRoomCommand, TResponse> next)
    {
        var response = await next.Invoke(command, ct);
        
        _gameRoomConcurrencyCheckEnforcer.ForceConcurrencyTokenCheckFor(command.GameRoomId);
        
        return response;
    }
}
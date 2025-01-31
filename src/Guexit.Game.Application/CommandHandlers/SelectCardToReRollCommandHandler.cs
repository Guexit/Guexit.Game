using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SelectCardToReRollCommandHandler : ICommandHandler<SelectCardToReRollCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SelectCardToReRollCommandHandler(IGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }
    
    public async ValueTask<Unit> Handle(SelectCardToReRollCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)  
            ?? throw new GameRoomNotFoundException(command.GameRoomId);
        
        gameRoom.SelectCardToReRoll(command.PlayerId, command.CardToReRollId, command.NewCardId);
        
        return Unit.Value;
    }
}
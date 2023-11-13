using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class SubmitGuessingPlayerCardCommandHandler : ICommandHandler<SubmitGuessingPlayerCardCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public SubmitGuessingPlayerCardCommandHandler(IGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<Unit> Handle(SubmitGuessingPlayerCardCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.SubmitGuessingPlayerCard(command.PlayerId, command.CardId);
        
        return Unit.Value;
    }
}

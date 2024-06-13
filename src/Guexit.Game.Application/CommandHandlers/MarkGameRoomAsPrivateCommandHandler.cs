using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class MarkGameRoomAsPrivateCommandHandler : ICommandHandler<MarkGameRoomAsPrivateCommand>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public MarkGameRoomAsPrivateCommandHandler(IGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<Unit> Handle(MarkGameRoomAsPrivateCommand command, CancellationToken ct = default)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.MarkAsPrivate(command.PlayerId);

        return Unit.Value;
    }
}

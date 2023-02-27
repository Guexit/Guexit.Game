using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class StartGameCommandHandler : CommandHandler<StartGameCommand, Unit>
{
    private readonly IGameRoomRepository _gameRoomRepository;

    public StartGameCommandHandler(IUnitOfWork unitOfWork, IGameRoomRepository gameRoomRepository) : base(unitOfWork)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask<Unit> Process(StartGameCommand command, CancellationToken cancellationToken)
    {
        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, cancellationToken);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.Start();

        return Unit.Value;
    }
}

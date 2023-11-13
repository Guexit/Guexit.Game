using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class CreateGameRoomCommandHandler : ICommandHandler<CreateGameRoomCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly ISystemClock _clock;

    public CreateGameRoomCommandHandler(IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository, ISystemClock clock)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
        _clock = clock;
    }

    public async ValueTask<Unit> Handle(CreateGameRoomCommand command, CancellationToken ct = default)
    {
        var playerCreatingTheGame = await _playerRepository.GetBy(command.PlayerId, ct);
        if (playerCreatingTheGame is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = new GameRoom(command.GameRoomId, playerCreatingTheGame.Id, _clock.UtcNow);
        await _gameRoomRepository.Add(gameRoom, ct);
        
        return Unit.Value;
    }
}

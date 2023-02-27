using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class CreateGameRoomCommandHandler : CommandHandler<CreateGameRoomCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly ISystemClock _clock;

    public CreateGameRoomCommandHandler(IUnitOfWork unitOfWork, IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository, ISystemClock clock) : base(unitOfWork)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
        _clock = clock;
    }

    protected override async ValueTask Process(CreateGameRoomCommand command, CancellationToken ct)
    {
        var playerCreatingTheGame = await _playerRepository.GetBy(command.PlayerId, ct);
        if (playerCreatingTheGame is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = new GameRoom(command.GameRoomId, playerCreatingTheGame.Id, _clock.UtcNow);
        await _gameRoomRepository.Add(gameRoom, ct);
    }
}

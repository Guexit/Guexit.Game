using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class JoinGameRoomCommandHandler : CommandHandler<JoinGameRoomCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public JoinGameRoomCommandHandler(IUnitOfWork unitOfWork, IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository) 
        : base(unitOfWork)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask Process(JoinGameRoomCommand command, CancellationToken ct)
    {
        var player = await _playerRepository.GetBy(command.PlayerId, ct);
        if (player is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.Join(player.Id);
    }
}

using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class JoinGameRoomCommandHandler : ICommandHandler<JoinGameRoomCommand>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;

    public JoinGameRoomCommandHandler(IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository) 

    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<Unit> Handle(JoinGameRoomCommand command, CancellationToken ct = default)
    {
        var player = await _playerRepository.GetBy(command.PlayerId, ct);
        if (player is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);

        gameRoom.Join(player.Id);
        
        return Unit.Value;
    }
}

using Mediator;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Application.Exceptions;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.CommandHandlers;

public sealed class JoinGameRoomCommandHandler : CommandHandler<JoinGameRoomCommand, Unit>
{
    private IPlayerRepository _playerRepository;
    private IGameRoomRepository _gameRoomRepository;

  
    public JoinGameRoomCommandHandler(IUnitOfWork unitOfWork, IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository) 
        : base(unitOfWork)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
    }

    protected override async ValueTask<Unit> Process(JoinGameRoomCommand command, CancellationToken ct)
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

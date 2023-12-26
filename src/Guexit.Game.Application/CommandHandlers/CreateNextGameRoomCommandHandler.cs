using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public sealed class CreateNextGameRoomCommandHandler : ICommandHandler<CreateNextGameRoomCommand, GameRoomId>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly ISystemClock _clock;

    public CreateNextGameRoomCommandHandler(IPlayerRepository playerRepository, IGameRoomRepository gameRoomRepository,
        IGuidProvider guidProvider, ISystemClock clock)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
        _guidProvider = guidProvider;
        _clock = clock;
    }

    public async ValueTask<GameRoomId> Handle(CreateNextGameRoomCommand command, CancellationToken ct = default)
    {
        var player = await _playerRepository.GetBy(command.PlayerId, ct);
        if (player is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = await _gameRoomRepository.GetBy(command.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(command.GameRoomId);

        if (gameRoom.IsLinkedToNextGameRoom())
            return gameRoom.NextGameRoomId;
        
        var newGameRoom = new GameRoom(_guidProvider.NewGuid(), command.PlayerId, _clock.UtcNow);
        gameRoom.LinkToNextGameRoom(newGameRoom.Id);
        
        await _gameRoomRepository.Add(newGameRoom, ct);
        
        return gameRoom.NextGameRoomId;
    }
}
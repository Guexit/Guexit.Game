using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Application.Exceptions;
using TryGuessIt.Game.Domain;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.CommandHandlers;

public sealed class CreateGameRoomCommandHandler : CommandHandler<CreateGameRoomCommand, CreateGameRoomCommandCompletion>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly ISystemClock _systemClock;

    public CreateGameRoomCommandHandler(
        IUnitOfWork unitOfWork, 
        IPlayerRepository playerRepository, 
        IGameRoomRepository gameRoomRepository,
        IGuidProvider guidProvider, 
        ISystemClock systemClock) : base(unitOfWork)
    {
        _playerRepository = playerRepository;
        _gameRoomRepository = gameRoomRepository;
        _guidProvider = guidProvider;
        _systemClock = systemClock;
    }

    protected override async ValueTask<CreateGameRoomCommandCompletion> Process(CreateGameRoomCommand command, CancellationToken ct)
    {
        var playerCreatingTheGame = await _playerRepository.GetById(command.PlayerId, ct);
        if (playerCreatingTheGame is null)
            throw new PlayerNotFoundException(command.PlayerId);

        var gameRoom = new GameRoom(new GameRoomId(_guidProvider.NewGuid()), playerCreatingTheGame.Id, _systemClock.UtcNow);
        await _gameRoomRepository.Add(gameRoom);
        return new CreateGameRoomCommandCompletion(gameRoom.Id);
    }
}

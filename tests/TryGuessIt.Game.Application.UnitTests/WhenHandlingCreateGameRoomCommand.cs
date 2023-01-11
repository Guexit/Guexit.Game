using NSubstitute;
using TryGuessIt.Game.Application.CommandHandlers;
using TryGuessIt.Game.Domain.Model.GameRoom;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public sealed class WhenHandlingCreateGameRoomCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly CreatePlayerCommandHandler _playerManagementService;

    public WhenHandlingCreateGameRoomCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _playerManagementService = new CreateGameRoomCommandHandler(
            Substitute.For<IUnitOfWork>(),
            _playerRepository,
            _gameRoomRepository
        );
    }
}

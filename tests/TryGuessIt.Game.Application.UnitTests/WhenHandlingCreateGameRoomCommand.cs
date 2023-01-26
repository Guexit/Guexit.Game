using TryGuessIt.Game.Application.CommandHandlers;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Application.Exceptions;
using TryGuessIt.Game.Domain;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public sealed class WhenHandlingCreateGameRoomCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly ISystemClock _systemClock;
    private readonly CreateGameRoomCommandHandler _commandHandler;

    public WhenHandlingCreateGameRoomCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _guidProvider = Substitute.For<IGuidProvider>();
        _systemClock = Substitute.For<ISystemClock>();
        _commandHandler = new CreateGameRoomCommandHandler(
            Substitute.For<IUnitOfWork>(),
            _playerRepository,
            _gameRoomRepository,
            _guidProvider,
            _systemClock
        );
    }

    [Fact]
    public async Task GameRoomIsCreated()
    {
        var playerId = "playerId";
        var command = new CreateGameRoomCommand(playerId);
        var gameRoomId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2022, 1, 1, 2, 3, 4, TimeSpan.Zero);
        _guidProvider.NewGuid().Returns(gameRoomId);
        _systemClock.UtcNow.Returns(createdAt);
        await _playerRepository.Add(new Player(new PlayerId(playerId), string.Empty));

        var completion = await _commandHandler.Handle(command);

        completion.Should().NotBeNull();
        completion!.GameRoomId.Should().Be(new GameRoomId(gameRoomId));

        var gameRoom = await _gameRoomRepository.GetBy(new GameRoomId(gameRoomId));
        gameRoom.Should().NotBeNull();
        gameRoom!.PlayerIds.Single().Should().Be(new PlayerId(playerId));
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
    }

    [Fact]
    public async Task ThrowsPlayerNotFoundException()
    {
        var command = new CreateGameRoomCommand("nonExistingPlayerId");

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<PlayerNotFoundException>();
    }
}

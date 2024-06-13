using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingCreateGameRoomCommand
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly ISystemClock _systemClock;
    private readonly CreateGameRoomCommandHandler _commandHandler;

    public WhenHandlingCreateGameRoomCommand()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _systemClock = Substitute.For<ISystemClock>();
        _commandHandler = new CreateGameRoomCommandHandler(_playerRepository, _gameRoomRepository, _systemClock);
    }

    [Fact]
    public async Task GameRoomIsCreated()
    {
        var gameRoomId = Guid.NewGuid();
        var playerId = new PlayerId("playerId");
        var command = new CreateGameRoomCommand(gameRoomId, playerId);
        var createdAt = new DateTimeOffset(2022, 1, 1, 2, 3, 4, TimeSpan.Zero);
        _systemClock.UtcNow.Returns(createdAt);
        await _playerRepository.Add(new Player(new PlayerId(playerId), "batman@acme.com"));

        await _commandHandler.Handle(command);

        var gameRoom = await _gameRoomRepository.GetBy(new GameRoomId(gameRoomId));
        gameRoom.Should().NotBeNull();
        gameRoom!.CreatedBy.Should().Be(playerId);
        gameRoom.PlayerIds.Single().Should().Be(playerId);
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.RequiredMinPlayers.Should().Be(RequiredMinPlayers.Default);
        gameRoom.IsPublic.Should().BeFalse();
    }

    [Fact]
    public async Task ThrowsPlayerNotFoundException()
    {
        var command = new CreateGameRoomCommand(Guid.NewGuid(), "nonExistingPlayerId");

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<PlayerNotFoundException>();
    }
}

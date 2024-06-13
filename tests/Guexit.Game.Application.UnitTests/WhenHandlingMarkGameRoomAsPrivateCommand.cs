using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingMarkGameRoomAsPrivateCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly MarkGameRoomAsPrivateCommandHandler _commandHandler;

    public WhenHandlingMarkGameRoomAsPrivateCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new MarkGameRoomAsPrivateCommandHandler(_gameRoomRepository);
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new MarkGameRoomAsPrivateCommand("anyPlayerId", nonExistingGameRoomId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsPlayerNotInGameRoomException()
    {
        var playerIdThatIsNotInGameRoom = new PlayerId("notFoundInGameRoom");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("player1"))
            .WithPlayersThatJoined(["player2", "player3"])
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new MarkGameRoomAsPrivateCommand(playerIdThatIsNotInGameRoom, GameRoomId));

        await action.Should().ThrowAsync<PlayerNotInGameRoomException>();
    }

    [Fact]
    public async Task ThrowsGamePermissionDeniedExceptionIfItsNotDoneByCreator()
    {
        var playerThatJoined = new PlayerId("player2");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("player1"))
            .WithPlayersThatJoined([playerThatJoined, "player3" ])
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new MarkGameRoomAsPrivateCommand(playerThatJoined, GameRoomId));

        await action.Should().ThrowAsync<GamePermissionDeniedException>();
    }

    [Fact]
    public async Task GameRoomIsMarkedAsPrivate()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .WithIsPublic(true)
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPrivateCommand(creatorPlayerId, GameRoomId));

        gameRoom.IsPublic.Should().BeFalse();
    }

    [Fact]
    public async Task GameRoomMarkedAsPrivateIsRaised()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .WithIsPublic(true)
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPrivateCommand(creatorPlayerId, GameRoomId));

        gameRoom.DomainEvents.OfType<GameRoomMarkedAsPrivate>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<GameRoomMarkedAsPrivate>().Single();
        @event.GameRoomId.Should().Be(gameRoom.Id);
    }

    [Fact]
    public async Task GameRoomMarkedAsPrivateIsNotRaisedIfItWasAlreadyPrivate()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithIsPublic(false)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPrivateCommand(creatorPlayerId, GameRoomId));

        gameRoom.DomainEvents.OfType<GameRoomMarkedAsPrivate>().Should().BeEmpty();
    }
}


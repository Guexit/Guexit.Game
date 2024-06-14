using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingMarkGameRoomAsPublicCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly MarkGameRoomAsPublicCommandHandler _commandHandler;

    public WhenHandlingMarkGameRoomAsPublicCommand()
    {

        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new MarkGameRoomAsPublicCommandHandler(_gameRoomRepository);
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new MarkGameRoomAsPublicCommand("anyPlayerId", nonExistingGameRoomId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsPlayerNotFoundInCurrentGuessingPlayersExceptionException()
    {
        var playerIdThatIsNotInGameRoom = new PlayerId("notFoundInGameRoom");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("player1"))
            .WithPlayersThatJoined(["player2", "player3"])
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(playerIdThatIsNotInGameRoom, GameRoomId));

        await action.Should().ThrowAsync<PlayerNotInGameRoomException>();
    }

    [Fact]
    public async Task ThrowsGamePermissionDeniedExceptionIfItsNotDoneByCreator()
    {
        var playerThatJoined = new PlayerId("player2");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("player1"))
            .WithPlayersThatJoined([playerThatJoined, "player3"])
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(playerThatJoined, GameRoomId));

        await action.Should().ThrowAsync<GamePermissionDeniedException>();
    }

    [Fact]
    public async Task ThrowsInvalidOperationForNotInProgressGameException()
    {
        var creator = new PlayerId("player1");
        await _gameRoomRepository.Add(GameRoomBuilder.CreateStarted(GameRoomId, creator, ["player2", "player3"])
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(creator, GameRoomId));

        await action.Should().ThrowAsync<InvalidOperationForStartedGameException>();
    }
    
    [Fact]
    public async Task GameRoomIsMarkedAsPublic()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(creatorPlayerId, GameRoomId));

        gameRoom.IsPublic.Should().BeTrue();
    }

    [Fact]
    public async Task GameRoomMarkedAsPublicEventIsRaised()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(creatorPlayerId, GameRoomId));

        gameRoom.DomainEvents.OfType<GameRoomMarkedAsPublic>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<GameRoomMarkedAsPublic>().Single();
        @event.GameRoomId.Should().Be(gameRoom.Id);
    }

    [Fact]
    public async Task GameRoomMarkedAsPublicEventIsNotRaisedIfItWasAlreadyPublic()
    {
        var creatorPlayerId = new PlayerId("player1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .WithIsPublic(true)
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new MarkGameRoomAsPublicCommand(creatorPlayerId, GameRoomId));

        gameRoom.DomainEvents.OfType<GameRoomMarkedAsPublic>().Should().BeEmpty();
    }
}

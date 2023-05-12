using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingStartGameCommand
{
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly StartGameCommandHandler _commandHandler;

    public WhenHandlingStartGameCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new StartGameCommandHandler(
            Substitute.For<IUnitOfWork>(),
            _gameRoomRepository
        );
    }

    [Fact]
    public async Task GameRoomChangesStatusToAssigningCardsAndAssignsFirstStoryTeller()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var creatorId = new PlayerId("1");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build());

        await _commandHandler.Handle(new StartGameCommand(gameRoomId, creatorId));

        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Status.Should().Be(GameStatus.AssigningCards);
        gameRoom.CurrentStoryTeller.PlayerId.Should().Be(creatorId);
        gameRoom.CurrentStoryTeller.Story.Should().Be(string.Empty);
    }

    [Fact]
    public async Task GameStartedDomainEventIsRaised()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var creatorId = new PlayerId("1");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build());

        await _commandHandler.Handle(new StartGameCommand(gameRoomId, creatorId));

        var gameRoom = await _gameRoomRepository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.DomainEvents.Should().HaveCount(1);
        gameRoom.DomainEvents.Single().Should().BeEquivalentTo(new GameStarted(gameRoomId));
    }

    [Fact]
    public async Task ThrowsInsufficientPlayersToStartGameExceptionOnLessPlayersThanRequired()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var creatorId = new PlayerId("1");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithMinRequiredPlayers(3)
            .Build());

        var action = async () => await _commandHandler.Handle(new StartGameCommand(gameRoomId, creatorId));

        await action.Should().ThrowAsync<InsufficientPlayersToStartGameException>()
            .WithMessage($"Game room {gameRoomId} requires a minimum of {RequiredMinPlayers.Default.Count} players to start, but only 1 players are present.");
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfGameRoomDoesNotExist()
    {
        var playerId = new PlayerId("1");
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(new StartGameCommand(nonExistingGameRoomId, playerId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }
}

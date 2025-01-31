using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Application.UnitTests.Repositories;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingReserveCardsForReRollCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ReserveCardsForReRollCommandHandler _commandHandler;
    
    public WhenHandlingReserveCardsForReRollCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _imageRepository = new FakeInMemoryImageRepository();
        _commandHandler = new ReserveCardsForReRollCommandHandler(_gameRoomRepository, _imageRepository);
    }

    [Fact]
    public async Task CardsAreReservedToPlayer()
    {
        var reRollingPlayerId = new PlayerId("rerollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [reRollingPlayerId, "player3"]).Build();
        await _gameRoomRepository.Add(gameRoom);

        var expectedReservedImages = new[] 
        {
            ImageBuilder.CreateValid().Build(), 
            ImageBuilder.CreateValid().Build(), 
            ImageBuilder.CreateValid().Build()
        };
        
        await _imageRepository.AddRange(expectedReservedImages);
        await _commandHandler.Handle(new ReserveCardsForReRollCommand(reRollingPlayerId, GameRoomId));

        var cardReRoll = gameRoom.CurrentCardReRolls.First();
        cardReRoll.PlayerId.Should().Be(reRollingPlayerId);
        cardReRoll.IsCompleted.Should().BeFalse();
        cardReRoll.ReservedCards.Select(x => x.Url).Should().BeEquivalentTo(expectedReservedImages.Select(x => x.Url));
    }

    [Fact]
    public async Task CardReRollReservedEventIsRaised()
    {
        var reRollingPlayerId = new PlayerId("rerollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [reRollingPlayerId, "player3"]).Build();
        await _gameRoomRepository.Add(gameRoom);

        await _imageRepository.AddRange(new[]
        {
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build()
        });
        await _commandHandler.Handle(new ReserveCardsForReRollCommand(reRollingPlayerId, GameRoomId));

        gameRoom.DomainEvents.OfType<CardReRollReserved>().Should().HaveCount(1);

        var @event = gameRoom.DomainEvents.OfType<CardReRollReserved>().First();
        @event.GameRoomId.Should().Be(gameRoom.Id);
        @event.PlayerId.Should().Be(reRollingPlayerId);
    }
    
    [Fact]
    public async Task ThrowsGameRoomNotFoundException()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        var command = new ReserveCardsForReRollCommand("anyPlayerId", nonExistingGameRoomId);

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerNotInGameRoomException()
    {
        var nonExistingPlayerInGameRoom = new PlayerId("nonExistingPlayerId");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "player1", ["player2", "player3"]).Build();

        await _imageRepository.AddRange(Enumerable.Range(0, 3).Select(_ => ImageBuilder.CreateValid().Build()));
        await _gameRoomRepository.Add(gameRoom);

        var action = async () => await _commandHandler.Handle(new ReserveCardsForReRollCommand(nonExistingPlayerInGameRoom, GameRoomId));

        await action.Should().ThrowAsync<PlayerNotInGameRoomException>();
    }

    [Fact]
    public async Task ThrowsInsufficientCardsAvailableToReRollException()
    {
        var totalAvailableImages = CardReRoll.RequiredReservedCardsSize - 1;
        
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [reRollingPlayerId, "player3"]).Build();

        await _imageRepository.AddRange(Enumerable.Range(0, totalAvailableImages).Select(_ => ImageBuilder.CreateValid().Build()));
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new ReserveCardsForReRollCommand(reRollingPlayerId, GameRoomId));

        await action.Should().ThrowAsync<InsufficientCardsAvailableToReRollException>();
    }
    
    [Fact]
    public async Task ThrowsOnlyOneReRollAvailablePerRoundException()
    {
        var playerId = new PlayerId("rerollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [playerId, "player3"])
            .WithPlayerThatReservedCardsForReRoll(playerId)
            .Build();
        
        await _imageRepository.AddRange(Enumerable.Range(0, 3).Select(_ => ImageBuilder.CreateValid().Build()));
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new ReserveCardsForReRollCommand(playerId, GameRoomId));

        await action.Should().ThrowAsync<OnlyOneReRollAvailablePerRoundException>();
    }

    [Fact]
    public async Task MarksTemporarilyImagesAsUsedInGameRoomToAvoidShowingThemToOtherPlayers()
    {
        var playerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", [playerId, "player3"]).Build();

        var imagesToBeReserved = new[]
        {
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build(),
            ImageBuilder.CreateValid().Build()
        };

        await _imageRepository.AddRange(imagesToBeReserved);
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new ReserveCardsForReRollCommand(playerId, GameRoomId));

        imagesToBeReserved.Should().AllSatisfy(image =>
        {
            image.IsAssignedToAGameRoom.Should().BeTrue();
            image.GameRoomId.Should().Be(GameRoomId);
        });
    }

    [Fact]
    public async Task ThrowsGameNotInProgressException()
    {
        var playerId = new PlayerId("reRollingPlayer");
        
        var notStartedGameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(playerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .Build();

        await _imageRepository.AddRange(Enumerable.Range(0, 3).Select(_ => ImageBuilder.CreateValid().Build()));
        await _gameRoomRepository.Add(notStartedGameRoom);

        var action = async () => await _commandHandler.Handle(new ReserveCardsForReRollCommand(playerId, GameRoomId));

        await action.Should().ThrowAsync<InvalidOperationForNotInProgressGameException>();
    }
}

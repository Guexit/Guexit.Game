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

public sealed class WhenHandlingStartGameCommand
{
    private readonly GameRoomId GameRoomId = new(Guid.NewGuid());
    
    private readonly IImageRepository _imageRepository;
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly StartGameCommandHandler _commandHandler;

    public WhenHandlingStartGameCommand()
    {
        _imageRepository = new FakeInMemoryImageRepository();
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new StartGameCommandHandler(_gameRoomRepository, _imageRepository);
    }

    [Fact]
    public async Task GameRoomChangesStatusToInProgress()
    {
        var creatorId = new PlayerId("1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        await _imageRepository.AddRange(CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck()));
        
        await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));

        gameRoom.Should().NotBeNull();
        gameRoom.Status.Should().Be(GameStatus.InProgress);
    }

    [Theory]
    [InlineData(3, 6)]
    [InlineData(4, 12)]
    [InlineData(5, 20)]
    [InlineData(6, 30)]
    [InlineData(7, 42)]
    [InlineData(8, 56)]
    [InlineData(9, 72)]
    [InlineData(10, 90)]
    public async Task AssignsDeckAndDealsInitialCardsToPlayers(int totalPlayers, int expectedCardsInDeckAfterInitialDeal)
    {
        var creatorId = new PlayerId("creator");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(Enumerable.Range(0, totalPlayers - 1).Select(x => new PlayerId(x.ToString())).ToArray())
            .Build();
        var imagesToAssign = CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck());

        await _gameRoomRepository.Add(gameRoom);
        await _imageRepository.AddRange(imagesToAssign);

        await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));

        gameRoom.Deck.Count.Should().Be(expectedCardsInDeckAfterInitialDeal);
        gameRoom.Deck.Select(x => x.Url).Should().BeSubsetOf(imagesToAssign.Select(x => x.Url));

        var imagesUrls = imagesToAssign.Select(x => x.Url).ToHashSet();
        foreach (var playerHand in gameRoom.PlayerHands)
        {
            playerHand.Cards.Should().HaveCount(GameRoom.PlayerHandSize)
                .And.Subject.Select(x => x.Url).Should().BeSubsetOf(imagesUrls);
        }
    }

    [Fact]
    public async Task AssignsFirstStoryTeller()
    {
        var creatorId = new PlayerId("1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        await _imageRepository.AddRange(CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck()));
        
        await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));

        gameRoom.Should().NotBeNull();
        gameRoom.CurrentStoryTeller.PlayerId.Should().Be(creatorId);
        gameRoom.CurrentStoryTeller.Story.Should().Be(string.Empty);
    }

    [Fact]
    public async Task GameStartedDomainEventIsRaised()
    {
        var creatorId = new PlayerId("1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .Build();
        await _imageRepository.AddRange(CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck()));
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));

        gameRoom.Should().NotBeNull();
        gameRoom.DomainEvents.OfType<GameStarted>().Should().HaveCount(1);
        gameRoom.DomainEvents.OfType<GameStarted>().Single().Should().BeEquivalentTo(new GameStarted(GameRoomId));
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfGameRoomDoesNotExist()
    {
        var playerId = new PlayerId("1");
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(new StartGameCommand(nonExistingGameRoomId, playerId));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsInsufficientImagesToAssignDeckExceptionIfNotEnoughImagesToStartGame()
    {
        var playerId = new PlayerId("creator");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(playerId)
            .WithPlayersThatJoined("thanos", "gamora", "thor")
            .Build();
        var insufficientAvailableCardsCount = gameRoom.GetRequiredNumberOfCardsInDeck() - 1;
        
        await _imageRepository.AddRange(CreateImages(insufficientAvailableCardsCount));
        await _gameRoomRepository.Add(gameRoom);

        var action = async () => await _commandHandler.Handle(new StartGameCommand(GameRoomId, playerId));

        await action.Should().ThrowAsync<InsufficientImagesToAssignDeckException>()
            .WithMessage($"{insufficientAvailableCardsCount} are less images than required to assign a deck to game room with id {GameRoomId.Value}");
    }

    [Fact]
    public async Task ThrowsInsufficientPlayersToStartGameExceptionOnLessPlayersThanRequired()
    {
        var creatorId = new PlayerId("1");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        await _imageRepository.AddRange(CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck()));
        
        var action = async () => await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));

        await action.Should().ThrowAsync<InsufficientPlayersToStartGameException>()
            .WithMessage($"Game room {GameRoomId.Value} requires a minimum of {RequiredMinPlayers.Default.Count} players to start, but only 1 players are present.");
    }

    [Fact]
    public async Task ImagesInAssignedDeckAreMarkedAsUsed()
    {
        var creatorId = new PlayerId("creator");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        var imagesToAssign = Enumerable.Range(0, gameRoom.GetRequiredNumberOfCardsInDeck())
            .Select(_ => ImageBuilder.CreateValid().Build())
            .ToArray();
        await _imageRepository.AddRange(imagesToAssign);

        await _commandHandler.Handle(new StartGameCommand(GameRoomId, creatorId));
        
        imagesToAssign.Should().AllSatisfy(image =>
        {
            image.IsAssignedToAGameRoom.Should().BeTrue();
            image.GameRoomId.Should().Be(gameRoom.Id);
        });
    }

    [Fact]
    public async Task ThrowsExceptionIfANonCreatorPlayerTriesToStartGame()
    {
        var creatorId = new PlayerId("creatorId");
        var nonCreatorId = new PlayerId("nonCreatorId");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(nonCreatorId, "otherPlayerId")
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        await _imageRepository.AddRange(CreateImages(gameRoom.GetRequiredNumberOfCardsInDeck()));

        var action = async () => await _commandHandler.Handle(new StartGameCommand(GameRoomId, nonCreatorId));

        await action.Should().ThrowAsync<GamePermissionDeniedException>();
    }

    private static Image[] CreateImages(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => ImageBuilder.CreateValid().Build())
            .ToArray();
    }
}

using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenAssigningDeck
{
    private static readonly GameRoomId GameRoomId = new(Guid.Parse("8681c4a6-ee24-412a-93cd-2dd75c7b91cf"));

    private readonly IImageRepository _imageRepository = new FakeInMemoryImageRepository();
    private readonly IGameRoomRepository _gameRoomRepository = new FakeInMemoryGameRoomRepository();
    private readonly IDeckAssignmentService _deckAssignmentService;

    public WhenAssigningDeck() => _deckAssignmentService = new DeckAssignmentService(_imageRepository, _gameRoomRepository);

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task ThrowsInsufficientImagesToAssignExceptionIfNotEnoughAvailableImages(int playersCount)
    {
        var insufficientAvailableCardsCount = (playersCount * GameRoom.TotalCardsPerPlayer) - 1;
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("creator"))
            .WithPlayersThatJoined(Enumerable.Range(0, playersCount - 1).Select(x => new PlayerId(x.ToString())).ToArray())
            .WithMinRequiredPlayers(3)
            .Build());

        var imageBuilder = new ImageBuilder();
        await _imageRepository.AddRange(Enumerable.Range(0, insufficientAvailableCardsCount)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        var action = async () => await _deckAssignmentService.AssignDeck(GameRoomId);

        await action.Should().ThrowAsync<InsufficientImagesToAssignDeckException>()
            .WithMessage($"{insufficientAvailableCardsCount} are less images than required to assign a deck to game room with id {GameRoomId.Value}");
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfGameRoomDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _deckAssignmentService.AssignDeck(nonExistingGameRoomId);

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task AssignsDeckToGameRoomAndDispatchesInitialCardsToPlayers()
    {
        var creatorId = new PlayerId("creator");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build());

        var playersInGameRoom = 4;
        var requiredCardsInDeck = playersInGameRoom * GameRoom.TotalCardsPerPlayer;
        var imageBuilder = new ImageBuilder();
        await _imageRepository.AddRange(Enumerable.Range(0, requiredCardsInDeck)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        await _deckAssignmentService.AssignDeck(GameRoomId);

        var gameRoom = await _gameRoomRepository.GetBy(GameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().HaveCount(requiredCardsInDeck - (GameRoom.PlayerHandSize * playersInGameRoom));
        gameRoom.Deck.Should().AllSatisfy(card => card.Url.ToString().Should().StartWith("https://pablocompany/image/"));
        gameRoom.Deck.Should().AllSatisfy(card => card.Url.ToString().Should().StartWith("https://pablocompany/image/"));

        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("creator")).Cards.Should().NotBeEmpty();
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("2")).Cards.Should().HaveCount(GameRoom.PlayerHandSize);
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("3")).Cards.Should().HaveCount(GameRoom.PlayerHandSize);
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("4")).Cards.Should().HaveCount(GameRoom.PlayerHandSize);
    }
    
    [Fact]
    public async Task DeckAssignedAndInitialCardsDealedDomainEventsAreRaised()
    {
        var creatorId = new PlayerId("creator");
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build());

        var playersInGameRoom = 4;
        var requiredCardsInDeck = playersInGameRoom * GameRoom.TotalCardsPerPlayer;
        var imageBuilder = new ImageBuilder();
        await _imageRepository.AddRange(Enumerable.Range(0, requiredCardsInDeck)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        await _deckAssignmentService.AssignDeck(GameRoomId);

        var gameRoom = await _gameRoomRepository.GetBy(GameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.DomainEvents.Should().NotBeEmpty();
        gameRoom.DomainEvents.OfType<InitialCardsDealed>().Should().HaveCount(1);
        gameRoom.DomainEvents.OfType<DeckAssigned>().Should().HaveCount(1);
    }

    [Fact]
    public async Task ImagesAreMarkedAsUsed()
    {
        var creatorId = new PlayerId("creator");
        var gameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build();
        await _gameRoomRepository.Add(gameRoom);

        var assignedImageIds = Enumerable.Range(0, gameRoom.GetRequiredNumberOfCardsInDeck())
            .Select(_ => new ImageId(Guid.NewGuid())).ToArray();
        await _imageRepository.AddRange(assignedImageIds
            .Select(imageId => new ImageBuilder()
                .WithId(imageId)
                .WithUrl(new Uri($"https://pablocompany/image/{imageId.Value}"))
                .Build())
            .ToArray());

        await _deckAssignmentService.AssignDeck(GameRoomId);

        var assignedImages = new List<Image>(assignedImageIds.Length);
        foreach (var assignedImageId in assignedImageIds)
        {
            assignedImages.Add((await _imageRepository.GetBy(assignedImageId))!);
        }

        assignedImages.Should().AllSatisfy(image =>
        {
            image.IsAssignedToGameRoom.Should().BeTrue();
            image.GameRoomId.Should().Be(gameRoom.Id);
        });
    }
}

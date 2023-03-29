using Guexit.Game.Application.CardAssigment;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Application.Exceptions;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenAssigningDeck
{
    private const int LogicalShard = 27;
    private const int CardsPerPlayer = 8;
    private const int CardsInHandPerPlayer = 4;
    private static readonly GameRoomId GameRoomId = new GameRoomId(Guid.Parse("8681c4a6-ee24-412a-93cd-2dd75c7b91cf"));

    private readonly ILogicalShardProvider _logicalShardProvider = Substitute.For<ILogicalShardProvider>();
    private readonly IImageRepository _imageRepository = new FakeInMemoryImageRepository();
    private readonly IGameRoomRepository _gameRoomRepository = new FakeInMemoryGameRoomRepository();
    private readonly ILogicalShardDistributedLock _logicalShardDistributedLock = Substitute.For<ILogicalShardDistributedLock>();
    private readonly IDeckAssignmentService _deckAssignmentService;

    public WhenAssigningDeck()
    {
        _logicalShardProvider.GetLogicalShard().Returns(LogicalShard);
        _logicalShardDistributedLock.Acquire(LogicalShard).Returns(Task.CompletedTask);
        _logicalShardDistributedLock.Release(LogicalShard).Returns(Task.CompletedTask);

        _deckAssignmentService = new DeckAssignmentService(
            _logicalShardDistributedLock, 
            _logicalShardProvider,
            _imageRepository, 
            _gameRoomRepository);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task ThrowsInsufficientImagesToAssignExceptionIfNotEnoughAvailableImages(int numPlayers)
    {
        var insufficientAvailableCardsCount = (numPlayers * CardsPerPlayer) - 1;
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("creator"))
            .WithPlayersThatJoined(Enumerable.Range(0, numPlayers - 1).Select(x => new PlayerId(x.ToString())).ToArray())
            .WithMinRequiredPlayers(3)
            .Build());

        var imageBuilder = new ImageBuilder().WithLogicalShard(LogicalShard);
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
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(new PlayerId("creator"))
            .WithPlayersThatJoined(new PlayerId("2"), new PlayerId("3"), new PlayerId("4"))
            .WithMinRequiredPlayers(3)
            .Build());

        var playersInGameRoom = 4;
        var requiredCardsInDeck = playersInGameRoom * CardsPerPlayer;
        var imageBuilder = new ImageBuilder().WithLogicalShard(LogicalShard);
        await _imageRepository.AddRange(Enumerable.Range(0, requiredCardsInDeck)
            .Select(i => imageBuilder.WithId(Guid.NewGuid()).WithUrl(new Uri($"https://pablocompany/image/{i}")).Build())
            .ToArray());

        await _deckAssignmentService.AssignDeck(GameRoomId);

        var gameRoom = await _gameRoomRepository.GetBy(GameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().HaveCount(requiredCardsInDeck - (CardsInHandPerPlayer * playersInGameRoom));
        gameRoom.Deck.Should().AllSatisfy(card => card.Url.ToString().Should().StartWith("https://pablocompany/image/"));
        gameRoom.Deck.Should().AllSatisfy(card => card.Url.ToString().Should().StartWith("https://pablocompany/image/"));
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("creator")).Cards.Should().NotBeEmpty();
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("2")).Cards.Should().HaveCount(CardsInHandPerPlayer);
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("3")).Cards.Should().HaveCount(CardsInHandPerPlayer);
        gameRoom.PlayerHands.First(x => x.PlayerId == new PlayerId("4")).Cards.Should().HaveCount(CardsInHandPerPlayer);
    }
}

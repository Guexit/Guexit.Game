using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingSubmitGuessingPlayerCardCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly SubmitGuessingPlayerCardCommandHandler _commandHandler;

    public WhenHandlingSubmitGuessingPlayerCardCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new SubmitGuessingPlayerCardCommandHandler(Substitute.For<IUnitOfWork>(), _gameRoomRepository);
    }

    [Fact]
    public async Task CardIsSubmitted()
    {
        var guessingPlayerId = new PlayerId("guessingPlayerId");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { guessingPlayerId, new PlayerId("player3") })
            .WithStoryTellerCardStory("Any story")
            .Build();
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(guessingPlayerId.Value, GameRoomId.Value, card.Id.Value));

        await AssertGuessingPlayerSubmittedCardAndItIsRemovedFromHisHand(guessingPlayerId, card.Id, card.Url);
    }

    [Fact]
    public async Task ThrowsGameRoomNotFoundExceptionIfDoesNotExist()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        var action = async () => await _commandHandler.Handle(
            new SubmitGuessingPlayerCardCommand("anyPlayerId", nonExistingGameRoomId, Guid.NewGuid()));

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }

    [Fact]
    public async Task ThrowsCannotSubmitCardStoryIfGameRoomIsNotInProgressException()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var guessingPlayerId = new PlayerId("guessingPlayerId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator("storyTellerId")
            .WithPlayersThatJoined(guessingPlayerId, new PlayerId("player2"))
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(guessingPlayerId, gameRoomId, anyCardId));

        await action.Should().ThrowAsync<CannotSubmitCardIfGameRoomIsNotInProgressException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerNotFoundInCurrentGuessingPlayersExceptionExceptionIfStoryTellerTriesToSubmitGuessingCard()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var guessingPlayerId = new PlayerId("guessingPlayerId");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { guessingPlayerId, new PlayerId("player3") })
            .WithStoryTellerCardStory("Any story")
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(storyTellerId, GameRoomId, anyCardId));

        await action.Should().ThrowAsync<PlayerNotFoundInCurrentGuessingPlayersException>();
    }

    private async Task AssertGuessingPlayerSubmittedCardAndItIsRemovedFromHisHand(PlayerId playerId, CardId cardId, Uri url)
    {
        var gameRoom = await _gameRoomRepository.GetBy(GameRoomId);

        gameRoom.Should().NotBeNull();
        gameRoom!.SubmittedCards.Should().HaveCount(2);

        var card = gameRoom.SubmittedCards.First(x => x.Id == cardId);
        card.Id.Should().Be(cardId);
        card.Url.Should().Be(url);

        var playerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == playerId);
        playerHand.Cards.Should().HaveCount(GameRoom.CardsInHandPerPlayer - 1);

        gameRoom.DomainEvents.OfType<GuessingPlayerCardSubmitted>().Should().HaveCount(1)
            .And.Subject.Single().Should().BeEquivalentTo(new GuessingPlayerCardSubmitted(GameRoomId, playerId, cardId));
    }
}

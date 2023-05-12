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
        var guessingPlayerId = new PlayerId("lastPendingToSubmitCard");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { guessingPlayerId, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .Build();
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(guessingPlayerId.Value, GameRoomId.Value, card.Id.Value));

        await AssertGuessingPlayerSubmittedCardAndItIsRemovedFromHisHand(guessingPlayerId, card.Id, card.Url);
    }

    [Fact]
    public async Task AllPlayerCardsSubmittedEventIsRaisedWhenAllPlayersSubmittedTheCard()
    {
        var lastPlayerPendingToSubmitCard = new PlayerId("lastPendingToSubmitCard");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { lastPlayerPendingToSubmitCard, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard("player3")
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == lastPlayerPendingToSubmitCard).Cards.First();

        await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(lastPlayerPendingToSubmitCard.Value, GameRoomId.Value, card.Id.Value));

        gameRoom.DomainEvents.OfType<AllPlayerCardsSubmitted>().Should().HaveCount(1);
        gameRoom.DomainEvents.OfType<AllPlayerCardsSubmitted>().Single()
            .Should().BeEquivalentTo(new AllPlayerCardsSubmitted(GameRoomId));
    }

    [Fact]
    public async Task DoesNotRaiseAllPlayerCardsSubmittedEventIsRaisedIfAnyPlayerIsPendingToSubmitCard()
    {
        var guessingPlayerId = new PlayerId("lastPendingToSubmitCard");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "storyTellerId", new[] { guessingPlayerId, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .Build();
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
        await _gameRoomRepository.Add(gameRoom);

        await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(guessingPlayerId.Value, GameRoomId.Value, card.Id.Value));

        gameRoom.DomainEvents.OfType<AllPlayerCardsSubmitted>().Should().HaveCount(0);
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
        var guessingPlayerId = new PlayerId("lastPendingToSubmitCard");
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
        var guessingPlayerId = new PlayerId("lastPendingToSubmitCard");
        var anyCardId = new CardId(Guid.NewGuid());
        await _gameRoomRepository.Add(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { guessingPlayerId, new PlayerId("player3") })
            .WithStoryTellerStory("Any story")
            .Build());

        var action = async () =>
            await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(storyTellerId, GameRoomId, anyCardId));

        await action.Should().ThrowAsync<PlayerNotFoundInCurrentGuessingPlayersException>();
    }


    [Fact]
    public async Task ThrowsGuessingPlayerCannotSubmitCardIfStoryTellerHaventSubmitStoryException()
    {
        var guessingPlayerId = new PlayerId("lastPendingToSubmitCard");
        var gameRoom = GameRoomBuilder
            .CreateStarted(GameRoomId, "storyTellerId", new[] { guessingPlayerId, new PlayerId("player3") })
            .Build();
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
        await _gameRoomRepository.Add(gameRoom);

        var action = async () =>
            await _commandHandler.Handle(new SubmitGuessingPlayerCardCommand(guessingPlayerId, GameRoomId, card.Id.Value));

        await action.Should().ThrowAsync<GuessingPlayerCannotSubmitCardIfStoryTellerHaventSubmitStoryException>();
    }

    private async Task AssertGuessingPlayerSubmittedCardAndItIsRemovedFromHisHand(PlayerId playerId, CardId cardId, Uri url)
    {
        var gameRoom = await _gameRoomRepository.GetBy(GameRoomId);

        gameRoom.Should().NotBeNull();
        gameRoom!.SubmittedCards.Should().HaveCount(2);

        var submittedCard = gameRoom.SubmittedCards.First(x => x.Card.Id == cardId);
        submittedCard.PlayerId.Should().Be(playerId);

        var playerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == playerId);
        playerHand.Cards.Should().HaveCount(GameRoom.CardsInHandPerPlayer - 1);

        gameRoom.DomainEvents.OfType<GuessingPlayerCardSubmitted>().Should().HaveCount(1)
            .And.Subject.Single().Should().BeEquivalentTo(new GuessingPlayerCardSubmitted(GameRoomId, playerId, cardId));
    }
}

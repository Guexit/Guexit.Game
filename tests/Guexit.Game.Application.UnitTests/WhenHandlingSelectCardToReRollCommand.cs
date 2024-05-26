using Guexit.Game.Application.CommandHandlers;
using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingSelectCardToReRollCommand
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());
    
    private readonly IGameRoomRepository _gameRoomRepository;
    private readonly SelectCardToReRollCommandHandler _commandHandler;

    public WhenHandlingSelectCardToReRollCommand()
    {
        _gameRoomRepository = new FakeInMemoryGameRoomRepository();
        _commandHandler = new SelectCardToReRollCommandHandler(_gameRoomRepository);
    }
    
    [Fact]
    public async Task ThrowsGameRoomNotFoundException()
    {
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());
        var command = new SelectCardToReRollCommand("anyPlayerId", nonExistingGameRoomId, new CardId(Guid.NewGuid()), new CardId(Guid.NewGuid()));

        var action = async () => await _commandHandler.Handle(command);

        await action.Should().ThrowAsync<GameRoomNotFoundException>();
    }
    
    [Fact]
    public async Task ThrowsPlayerNotInGameRoomException()
    {
        var nonExistingPlayerInGameRoom = new PlayerId("nonExistingPlayerId");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, "player1", ["player2", "player3"]).Build();
        
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(nonExistingPlayerInGameRoom,
            gameRoom.Id, Guid.NewGuid(), Guid.NewGuid()));

        await action.Should().ThrowAsync<PlayerNotInGameRoomException>();
    }
    
    [Fact]
    public async Task ThrowsCardReRollNotReservedException()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll("player2")
            .Build();
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId,
            gameRoom.Id, Guid.NewGuid(), Guid.NewGuid()));

        await action.Should().ThrowAsync<CardReRollNotReservedException>();
    }
    
    [Fact]
    public async Task ThrowsInvalidOperationForNotInProgressGameException()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        
        var notStartedGameRoom = new GameRoomBuilder()
            .WithId(GameRoomId)
            .WithCreator(reRollingPlayerId)
            .WithPlayersThatJoined(["player2", "player3"])
            .Build();

        await _gameRoomRepository.Add(notStartedGameRoom);

        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, Guid.NewGuid(), Guid.NewGuid()));

        await action.Should().ThrowAsync<InvalidOperationForNotInProgressGameException>();
    }

    [Fact]
    public async Task ThrowsNewCardNotFoundInReservedCardsToReRollException()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var nonExistingCardToReRollId = new CardId(Guid.NewGuid());
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId)
            .Build();

        var cardToReRoll = gameRoom.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards.First();
        
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, cardToReRoll.Id, nonExistingCardToReRollId));

        await action.Should().ThrowAsync<NewCardNotFoundInReservedCardsToReRollException>();
    }
    
    [Fact]
    public async Task ThrowsCardNotFoundInPlayerHandException()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId)
            .Build();

        var nonExistingCardIdInPlayerHand = new CardId(Guid.NewGuid());
        var newCard = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == reRollingPlayerId).ReservedCards.First();
        
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, nonExistingCardIdInPlayerHand, newCard.Id));

        await action.Should().ThrowAsync<CardNotFoundInPlayerHandException>();
    }
    
    [Fact]
    public async Task SwapsCardInPlayersHand()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId)
            .Build();

        var cardToReRoll = gameRoom.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards.First();
        var selectedNewCard = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == reRollingPlayerId).ReservedCards.First();
        
        await _gameRoomRepository.Add(gameRoom);
        
        await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, cardToReRoll.Id, selectedNewCard.Id));

        var cardsInReRollingPlayerHand = gameRoom.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards;
        cardsInReRollingPlayerHand.Should().NotContain(cardToReRoll);
        cardsInReRollingPlayerHand.Should().Contain(selectedNewCard);
        
        gameRoom.CurrentCardReRolls.Should().HaveCount(1);
        var cardReRoll = gameRoom.CurrentCardReRolls.First();
        cardReRoll.PlayerId.Should().Be(reRollingPlayerId);
        cardReRoll.IsCompleted.Should().BeTrue();
        cardReRoll.ReservedCards.Should().BeEmpty();
    }

    [Fact]
    public async Task ReserveCardsForReRollDiscardedIsRaisedForNonSelectedCards()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId)
            .Build();

        var cardToReRoll = gameRoom.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards.First();
        
        var cardReRoll = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == reRollingPlayerId);
        var selectedNewCard = cardReRoll.ReservedCards.First();
        var expectedDiscardedCards = cardReRoll.ReservedCards.Except([selectedNewCard]);
        
        await _gameRoomRepository.Add(gameRoom);
        
        await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, cardToReRoll.Id, selectedNewCard.Id));

        gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().Should().HaveCount(1);
        var @event = gameRoom.DomainEvents.OfType<ReserveCardsForReRollDiscarded>().First();
        @event.GameRoomId.Should().Be(GameRoomId);
        @event.UnusedCardImageUrls.Should().BeEquivalentTo(expectedDiscardedCards.Select(x => x.Url));
    }
    
    [Fact]
    public async Task ThrowsReRollAlreadyCompletedThisRoundException()
    {
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId, completed: true)
            .Build();
        
        await _gameRoomRepository.Add(gameRoom);
        
        var action = async () => await _commandHandler.Handle(new SelectCardToReRollCommand(reRollingPlayerId, GameRoomId, Guid.NewGuid(), Guid.NewGuid()));

        await action.Should().ThrowAsync<ReRollAlreadyCompletedThisRoundException>();
    }
}
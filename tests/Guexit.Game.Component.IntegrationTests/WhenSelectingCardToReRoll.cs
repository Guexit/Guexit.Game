using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenSelectingCardToReRoll : ComponentTest
{    
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenSelectingCardToReRoll(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
    }
    
    [Fact]
    public async Task SwapsCardInPlayersHand()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var reRollingPlayerId = new PlayerId("reRollingPlayer");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, reRollingPlayerId, ["player2", "player3"])
            .WithPlayerThatReservedCardsForReRoll(reRollingPlayerId)
            .Build();

        var cardToReRoll = gameRoom.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards.First();
        var selectedNewCard = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == reRollingPlayerId).ReservedCards.First();
        
        await SaveInRepository(gameRoom);
        
        using var response = await Send(
            HttpMethod.Post, 
            $"game-rooms/{gameRoomId.Value}/player-hand/{cardToReRoll.Id.Value}/swap-with/{selectedNewCard.Id.Value}", 
            reRollingPlayerId.Value
        );
        await response.ShouldHaveSuccessStatusCode();
        
        await AssertCardHasReRolled(gameRoomId, reRollingPlayerId, cardToReRoll, selectedNewCard);
    }

    private async Task AssertCardHasReRolled(GameRoomId gameRoomId, PlayerId reRollingPlayerId, Card cardToReRoll, Card selectedNewCard)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();

        var gameRoom = await gameRoomRepository.GetBy(gameRoomId);

        gameRoom.Should().NotBeNull();
        
        var cardsInReRollingPlayerHand = gameRoom!.PlayerHands.First(x => x.PlayerId == reRollingPlayerId).Cards;
        cardsInReRollingPlayerHand.Should().NotContain(cardToReRoll);
        cardsInReRollingPlayerHand.Should().Contain(selectedNewCard);
        
        gameRoom.CurrentCardReRolls.Should().HaveCount(1);
        var cardReRoll = gameRoom.CurrentCardReRolls.First();
        cardReRoll.PlayerId.Should().Be(reRollingPlayerId);
        cardReRoll.IsCompleted.Should().BeTrue();
        cardReRoll.ReservedCards.Should().BeEmpty();
    }
}
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingCardsForReRoll : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingCardsForReRoll(GameWebApplicationFactory factory) : base(factory) { }


    [Fact]
    public async Task ReturnsCardsForReRollReadModel()
    {
        var thanos = new PlayerBuilder().WithId("thanos").WithUsername("thanos69@guexit.com").Build();
        var ironMan = new PlayerBuilder().WithId("ironman").WithUsername("ironman420@guexit.com").Build();
        var starLord = new PlayerBuilder().WithId("starlord").WithUsername("starlordxd@guexit.com").Build();
        await SaveInRepository(thanos, ironMan, starLord);
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, thanos.Id, [ironMan.Id, starLord.Id])
            .WithPlayerThatReservedCardsForReRoll(thanos.Id)
            .Build();
        await SaveInRepository(gameRoom);
        var thanosHand = gameRoom.PlayerHands.First(x => x.PlayerId == thanos.Id);
        var thanosReservedReRoll = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == thanos.Id);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/cards-for-re-roll", thanos.Id);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<CardReRollReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId.Value);

        var expectedPlayerHand = thanosHand.Cards.Select(x => new CardReRollReadModel.CardForReRollDto { Id = x.Id, Url = x.Url });
        readModel.CurrentPlayerHand.Should().BeEquivalentTo(expectedPlayerHand);

        var expectedReservedCardsForReRoll = thanosReservedReRoll.ReservedCards.Select(x => new CardReRollReadModel.CardForReRollDto { Id = x.Id, Url = x.Url });
        readModel.ReservedCardsToReRoll.Should().BeEquivalentTo(expectedReservedCardsForReRoll);
    }
}

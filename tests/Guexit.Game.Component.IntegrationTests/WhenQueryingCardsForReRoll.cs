using System.Net;
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingCardsForReRoll : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingCardsForReRoll(GameWebApplicationFactory factory) : base(factory) { }


    [Fact]
    public async Task ReturnsCardsForReRollReadModel()
    {
        var thanos = new PlayerId("thanos");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, thanos, ["ironman", "starlord"])
            .WithPlayerThatReservedCardsForReRoll("ironman")
            .WithPlayerThatReservedCardsForReRoll(thanos)
            .Build();
        await SaveInRepository(gameRoom);
        var thanosHand = gameRoom.PlayerHands.First(x => x.PlayerId == thanos);
        var thanosReservedReRoll = gameRoom.CurrentCardReRolls.First(x => x.PlayerId == thanos);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/cards-for-re-roll", thanos.Value);
        await response.ShouldHaveSuccessStatusCode();

        var readModel = await response.Content.ReadFromJsonAsync<CardReRollReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId.Value);

        var expectedPlayerHand = thanosHand.Cards.Select(x => new CardReRollReadModel.CardForReRollDto { Id = x.Id, Url = x.Url });
        readModel.CurrentPlayerHand.Should().BeEquivalentTo(expectedPlayerHand);

        var expectedReservedCardsForReRoll = thanosReservedReRoll.ReservedCards.Select(x => new CardReRollReadModel.CardForReRollDto { Id = x.Id, Url = x.Url });
        readModel.ReservedCardsToReRoll.Should().BeEquivalentTo(expectedReservedCardsForReRoll);
    }

    [Fact]
    public async Task ReturnsNotFoundIfGameRoomDoesNotExist()
    {
        var thanos = new PlayerId("thanos");
        var nonExistingGameRoomId = new GameRoomId(Guid.NewGuid());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/cards-for-re-roll", thanos);

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnsBadRequestIfNoCardsForReRollWereReserved()
    {
        var thanos = new PlayerId("thanos");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, thanos, ["ironman", "starlord"]).Build();
        await SaveInRepository(gameRoom);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/cards-for-re-roll", thanos.Value);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReturnsBadRequestIfCardsForReRollWasAlreadyCompleted()
    {
        var thanos = new PlayerId("thanos");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, thanos, ["ironman", "starlord"])
            .WithPlayerThatReservedCardsForReRoll(thanos, completed: true)
            .Build();
        await SaveInRepository(gameRoom);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/cards-for-re-roll", thanos.Value);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }
}

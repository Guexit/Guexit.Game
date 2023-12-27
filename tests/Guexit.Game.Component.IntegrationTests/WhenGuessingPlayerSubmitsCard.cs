using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.WebApi.Contracts.Requests;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenGuessingPlayerSubmitsCard : ComponentTest
{
    public WhenGuessingPlayerSubmitsCard(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task CardIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var guessingPlayerId = new PlayerId("player2");

        await Save(
            new PlayerBuilder().WithId("storyTellerId").Build(),
            new PlayerBuilder().WithId("player2").Build(),
            new PlayerBuilder().WithId("player3").Build()
        );

        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, "storyTellerId", [guessingPlayerId, "player3"])
            .WithStoryTellerStory("Any card story")
            .Build();

        await Save(gameRoom);
        
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();

        var submitCardResponse = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoomId.Value}/guessing-player/submit-card", 
            JsonContent.Create(new SubmitCardForGuessingPlayerRequest(card.Id.Value)), 
            guessingPlayerId
        );
        await submitCardResponse.ShouldHaveSuccessStatusCode();

        var getBoardResponse = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", guessingPlayerId);
        await getBoardResponse.ShouldHaveSuccessStatusCode();

        var boardReadModel = await getBoardResponse.Content.ReadFromJsonAsync<BoardReadModel>();
        boardReadModel.Should().NotBeNull();
        boardReadModel!.CurrentUserSubmittedCard.Should().NotBeNull();
        boardReadModel.CurrentUserSubmittedCard!.Id.Should().Be(card.Id);
        boardReadModel.CurrentUserSubmittedCard.Url.Should().Be(card.Url);
    }
}

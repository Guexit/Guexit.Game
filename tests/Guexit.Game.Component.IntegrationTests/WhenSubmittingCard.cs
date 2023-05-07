using System.Net;
using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.WebApi.Requests;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenSubmittingCard : ComponentTest
{
    public WhenSubmittingCard(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task CardIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var guessingPlayerId = new PlayerId("player2");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, "storyTellerId", new PlayerId[] { guessingPlayerId, "player3" })
            .WithStoryTellerCardStory("Any card story")
            .Build();
        await Save(gameRoom);
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();

        var request = new HttpRequestMessage(HttpMethod.Post, $"/game-rooms/{gameRoomId.Value}/guessing-player/submit-card")
        {
            Content = JsonContent.Create(new SubmitCardForGuessingPlayerRequest(card.Id.Value))
        };
        request.AddPlayerIdHeader(guessingPlayerId);
        var client = WebApplicationFactory.CreateClient();
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: await response.Content.ReadAsStringAsync());

        var getBoardRequest = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        getBoardRequest.AddPlayerIdHeader(guessingPlayerId);
        var getBoardResponse = await client.SendAsync(getBoardRequest);

        var responseContent = await getBoardResponse.Content.ReadFromJsonAsync<GameBoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.SubmittedCard.Should().NotBeNull();
        responseContent.SubmittedCard!.Id.Should().Be(card.Id);
    }
}

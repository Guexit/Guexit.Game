using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.WebApi.Contracts.Requests;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenStorytellerSubmitsCardStory : ComponentTest
{
    public WhenStorytellerSubmitsCardStory(GameWebApplicationFactory factory) : base(factory)
    { }


    [Fact]
    public async Task CardStoryIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerPlayerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        const string story = "El tipico abuelo adolescente";
        
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, new[] { playerId2, playerId3 }).Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(storyTellerId).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        });

        var selectedCardId = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First().Id;

        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"/game-rooms/{gameRoom.Id.Value}/storyteller/submit-card-story")
        {
            Content = JsonContent.Create(new SubmitStoryTellerCardStoryRequest(selectedCardId, story))
        };
        request.AddPlayerIdHeader(storyTellerId);
        var response = await client.SendAsync(request);
        await response.ShouldHaveSuccessStatusCode();
        
        var getBoardRequest = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board");
        getBoardRequest.AddPlayerIdHeader(storyTellerId);
        var getBoardResponse = await client.SendAsync(getBoardRequest);
        await getBoardResponse.ShouldHaveSuccessStatusCode();

        var responseContent = await getBoardResponse.Content.ReadFromJsonAsync<GameBoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.CurrentStoryTeller.PlayerId.Should().Be(storyTellerId);
        responseContent.CurrentStoryTeller.Story.Should().Be(story);
        responseContent.CurrentStoryTeller.Username.Should().Be("gamora");
        responseContent.SubmittedCards.Should().HaveCount(1);
        responseContent.SubmittedCards[0].Id.Should().Be(selectedCardId);
    }
}

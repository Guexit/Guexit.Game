using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.WebApi.Contracts.Requests;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenStorytellerSubmitsCardAndStory : ComponentTest
{
    public WhenStorytellerSubmitsCardAndStory(GameWebApplicationFactory factory) : base(factory)
    { }


    [Fact]
    public async Task CardStoryIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var storyTellerId = new PlayerId("storyTellerPlayerId");
        var playerId2 = new PlayerId("player2");
        var playerId3 = new PlayerId("player3");
        var story = "El tipico abuelo adolescente";
        
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, storyTellerId, [playerId2, playerId3]).Build();
        await Save(gameRoom);
        await Save(
        [
            new PlayerBuilder().WithId(storyTellerId).WithUsername("gamora").Build(),
            new PlayerBuilder().WithId(playerId2).WithUsername("starlord").Build(),
            new PlayerBuilder().WithId(playerId3).WithUsername("wroot").Build()
        ]);

        var selectedCardId = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First().Id;

        using var response = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoom.Id.Value}/storyteller/submit-card-story", 
            JsonContent.Create(new SubmitStoryTellerCardStoryRequest(selectedCardId, story)), 
            storyTellerId
        );
        await response.ShouldHaveSuccessStatusCode();
        
        using var getBoardResponse = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/board", storyTellerId);
        await getBoardResponse.ShouldHaveSuccessStatusCode();

        var responseContent = await getBoardResponse.Content.ReadFromJsonAsync<BoardReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.CurrentStoryTeller.PlayerId.Should().Be(storyTellerId);
        responseContent.CurrentStoryTeller.Story.Should().Be(story);
        responseContent.CurrentStoryTeller.Username.Should().Be("gamora");
    }
}

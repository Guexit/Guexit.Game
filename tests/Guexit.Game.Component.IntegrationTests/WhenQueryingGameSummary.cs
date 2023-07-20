using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameSummary : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingGameSummary(GameWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task ReturnsGameSummaryReadModel()
    {
        var storyTellerId = new PlayerId("thanos");
        var guessingPlayer1 = new PlayerId("ironman");
        var guessingPlayer2 = new PlayerId("starlord");
        var story = "Infinity gems";
        await Save(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { guessingPlayer1, guessingPlayer2 })
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1, guessingPlayer2)
            .WithVote(guessingPlayer1, storyTellerId)
            .WithVote(guessingPlayer2, storyTellerId)
            .WithEmptyDeck()
            .Build());
        await Save(
            new PlayerBuilder().WithId(storyTellerId).WithUsername("thanos").Build(),
            new PlayerBuilder().WithId(guessingPlayer1).WithUsername("ironman").Build(),
            new PlayerBuilder().WithId(guessingPlayer2).WithUsername("starlord").Build());

        using var client = WebApplicationFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/summary");
        request.AddPlayerIdHeader(storyTellerId);
        using var response = await client.SendAsync(request);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId);
        readModel.RoundSummaries.Should().HaveCount(1);
        readModel.Scores.Should().HaveCount(3);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == storyTellerId);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == guessingPlayer1);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == guessingPlayer2);
    }
}

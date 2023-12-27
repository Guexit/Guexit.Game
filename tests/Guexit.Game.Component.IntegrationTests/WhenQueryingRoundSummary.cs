using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingRoundSummary : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingRoundSummary(GameWebApplicationFactory factory) : base(factory) { }


    [Fact]
    public async Task ReturnsRoundSummaryReadModel()
    {
        var storyTellerId = new PlayerId("thanos");
        var guessingPlayer1 = new PlayerId("ironman");
        var guessingPlayer2 = new PlayerId("starlord");
        var story = "Infinity gems";
        await Save(
            new PlayerBuilder().WithId(storyTellerId).WithUsername("thanos").Build(),
            new PlayerBuilder().WithId(guessingPlayer1).WithUsername("ironman").Build(),
            new PlayerBuilder().WithId(guessingPlayer2).WithUsername("starlord").Build());

        await Save(GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [guessingPlayer1, guessingPlayer2])
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1, guessingPlayer2)
            .WithVote(guessingPlayer1, storyTellerId)
            .WithVote(guessingPlayer2, storyTellerId)
            .Build());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/round-summaries/last", storyTellerId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<RoundSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId.Value);
        readModel.StoryTeller.PlayerId.Should().Be(storyTellerId);
        readModel.StoryTeller.Username.Should().Be("thanos");
        readModel.StoryTeller.Story.Should().Be(story);
        readModel.SubmittedCardSummaries.Should().HaveCount(3);
        readModel.SubmittedCardSummaries.Single(x => x.SubmittedBy.PlayerId == storyTellerId.Value)
            .Voters.Should().HaveCount(2)
            .And.Subject.Should().Contain(x => x.PlayerId == guessingPlayer1.Value && x.Username == "ironman")
            .And.Subject.Should().Contain(x => x.PlayerId == guessingPlayer2.Value && x.Username == "starlord"); 

        readModel.Scores.Should().HaveCount(3);
        readModel.Scores.Single(x => x.Player.PlayerId == storyTellerId).Points.Should().Be(0);
        readModel.Scores.Single(x => x.Player.PlayerId == guessingPlayer1).Points.Should().Be(3);
        readModel.Scores.Single(x => x.Player.PlayerId == guessingPlayer2).Points.Should().Be(3);
    }
}

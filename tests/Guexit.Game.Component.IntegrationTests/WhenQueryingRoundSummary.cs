using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingRoundSummary : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingRoundSummary(GameWebApplicationFactory factory) : base(factory) { }


    [Fact]
    public async Task ReturnsRoundSummaryReadModel()
    {
        var thanos = new PlayerBuilder().WithId("thanos").WithUsername("thanos69@guexit.com").Build();
        var ironMan = new PlayerBuilder().WithId("ironman").WithUsername("ironman420@guexit.com").Build();
        var starLord = new PlayerBuilder().WithId("starlord").WithUsername("starlordxd@guexit.com").Build();
        var story = "Infinity gems";
        await Save(thanos, ironMan, starLord);
        await Save(GameRoomBuilder.CreateStarted(GameRoomId, thanos.Id, [ironMan.Id, starLord.Id])
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(ironMan.Id, starLord.Id)
            .WithVote(ironMan.Id, thanos.Id)
            .WithVote(starLord.Id, thanos.Id)
            .Build());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/round-summaries/last", thanos.Id);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<RoundSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId.Value);
        readModel.StoryTeller.PlayerId.Should().Be(thanos.Id);
        readModel.StoryTeller.Username.Should().Be(thanos.Username);
        readModel.StoryTeller.Nickname.Should().Be(thanos.Nickname.Value);
        readModel.StoryTeller.Story.Should().Be(story);
        readModel.SubmittedCardSummaries.Should().HaveCount(3);
        readModel.SubmittedCardSummaries.Single(x => x.SubmittedBy.PlayerId == thanos.Id.Value).Voters.Should().HaveCount(2)
            .And.Subject.Should().Contain(x => x.PlayerId == ironMan.Id.Value && x.Username == ironMan.Username && x.Nickname == ironMan.Nickname.Value)
            .And.Subject.Should().Contain(x => x.PlayerId == starLord.Id.Value && x.Username == starLord.Username && x.Nickname == starLord.Nickname.Value); 

        readModel.Scores.Should().HaveCount(3);
        readModel.Scores.Single(x => x.Player.PlayerId == thanos.Id).Points.Should().Be(0);
        readModel.Scores.Single(x => x.Player.PlayerId == ironMan.Id).Points.Should().Be(3);
        readModel.Scores.Single(x => x.Player.PlayerId == starLord.Id).Points.Should().Be(3);
    }
}

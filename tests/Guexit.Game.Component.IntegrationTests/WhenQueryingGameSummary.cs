using System.Net.Http.Json;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;
using Guexit.Game.Tests.Common.ObjectMothers;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameSummary : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingGameSummary(GameWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task ReturnsGameSummaryReadModel()
    {
        var thanos = new PlayerBuilder().WithId("thanos").WithUsername("thanos@guexit.com").Build();
        var ironMan = new PlayerBuilder().WithId("ironman").WithUsername("ironman@guexit.com").Build();
        var starLord = new PlayerBuilder().WithId("starlord").WithUsername("starlord@guexit.com").Build();
        
        var firstRoundStory = "Infinity gems";
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, thanos.Id, [ironMan.Id, starLord.Id])
            .WithStoryTellerStory(firstRoundStory)
            .WithGuessingPlayerThatSubmittedCard(ironMan.Id, starLord.Id)
            .WithVote(ironMan.Id, thanos.Id)
            .WithVote(starLord.Id, thanos.Id)
            .Build();
        AssumeStoryTellerSubmittedStory(gameRoom);
        AssumeAllPlayersSubmittedCard(gameRoom);
        AssumeAllGuessersVoted(gameRoom);
        
        await Save(gameRoom);
        await Save(thanos, ironMan, starLord);

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/summary", thanos.Id);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId);
        
        readModel.RoundSummaries.Should().HaveCount(2).And.BeInAscendingOrder(x => x.RoundFinishedAt);
        
        var firstRound = readModel.RoundSummaries.First();
        firstRound.StoryTeller.Story.Should().Be(firstRoundStory);
        firstRound.StoryTeller.Username.Should().Be(thanos.Username);
        firstRound.StoryTeller.PlayerId.Should().Be(thanos.Id.Value);
        firstRound.StoryTeller.Nickname.Should().Be(thanos.Nickname.Value);
        
        readModel.Scores.Should().HaveCount(3);
        readModel.Scores.Should().BeInDescendingOrder(x => x.Points);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == thanos.Id);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == ironMan.Id);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == starLord.Id);

        var thanosFirstRoundCard = firstRound.SubmittedCardSummaries.First(x => x.SubmittedBy.PlayerId == thanos.Id);
        thanosFirstRoundCard.SubmittedBy.Username.Should().Be(thanos.Username);
        thanosFirstRoundCard.SubmittedBy.Nickname.Should().Be(thanos.Nickname.Value);
        thanosFirstRoundCard.Voters.Should().BeEquivalentTo(new[]
        {
            new PlayerDto { Nickname = starLord.Nickname.Value, PlayerId = starLord.Id, Username = starLord.Username },
            new PlayerDto { Nickname = ironMan.Nickname.Value, PlayerId = ironMan.Id, Username = ironMan.Username }
        });
        
        var thanosScore = firstRound.Scores.First(x => x.Player.PlayerId == thanos.Id);
        thanosScore.Player.Username.Should().Be(thanos.Username);
        thanosScore.Player.Nickname.Should().Be(thanos.Nickname.Value);
        
        readModel.IsNextGameRoomLinked.Should().BeFalse();
        readModel.NextGameRoomId.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task ReturnsIfNextGameRoomIsCreatedItsId()
    {
        var playerId = new PlayerId("thanos");
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var gameRoom = GameRoomObjectMother.Finished(GameRoomId, playerId, ["ironman", "gamora"], nextGameRoomId);
        await Save(gameRoom);
        await Save(
            new PlayerBuilder().WithId(playerId).WithUsername("thanos").Build(),
            new PlayerBuilder().WithId("ironman").WithUsername("ironman").Build(),
            new PlayerBuilder().WithId("gamora").WithUsername("gamora").Build());
        
        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/summary", playerId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId);
        readModel.IsNextGameRoomLinked.Should().BeTrue();
        readModel.NextGameRoomId.Should().Be(nextGameRoomId);
    }
    
    private static void AssumeStoryTellerSubmittedStory(GameRoom gameRoom)
    {
        var storyTellerId = gameRoom.CurrentStoryTeller.PlayerId;
        var card = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First();

        gameRoom.SubmitStory(storyTellerId, card.Id, "Any story");
    }

    private static void AssumeAllPlayersSubmittedCard(GameRoom gameRoom)
    {
        foreach (var guessingPlayerId in gameRoom.GetCurrentGuessingPlayerIds())
        {
            var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
            gameRoom.SubmitGuessingPlayerCard(guessingPlayerId, card.Id);
        }
    }

    private static void AssumeAllGuessersVoted(GameRoom gameRoom)
    {
        foreach (var guessingPlayerId in gameRoom.GetCurrentGuessingPlayerIds())
        {
            var otherGuessingPlayerId = gameRoom.GetCurrentGuessingPlayerIds().First(x => x != guessingPlayerId);
            var submittedCard = gameRoom.SubmittedCards.First(x => x.PlayerId == otherGuessingPlayerId);
            gameRoom.VoteCard(guessingPlayerId, submittedCard.Card.Id);
        }
    }
}

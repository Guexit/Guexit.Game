using System.Net.Http.Json;
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
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, [guessingPlayer1, guessingPlayer2])
            .WithStoryTellerStory("Infinity gems")
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1, guessingPlayer2)
            .WithVote(guessingPlayer1, storyTellerId)
            .WithVote(guessingPlayer2, storyTellerId)
            .Build();
        AssumeStoryTellerSubmittedStory(gameRoom);
        AssumeAllPlayersSubmittedCard(gameRoom);
        AssumeAllGuessersVoted(gameRoom);
        await Save(gameRoom);
        await Save(
            new PlayerBuilder().WithId(storyTellerId).WithUsername("thanos").Build(),
            new PlayerBuilder().WithId(guessingPlayer1).WithUsername("ironman").Build(),
            new PlayerBuilder().WithId(guessingPlayer2).WithUsername("starlord").Build());

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{GameRoomId.Value}/summary", storyTellerId);

        await response.ShouldHaveSuccessStatusCode();
        var readModel = await response.Content.ReadFromJsonAsync<GameSummaryReadModel>();
        readModel.Should().NotBeNull();
        readModel!.GameRoomId.Should().Be(GameRoomId);
        readModel.RoundSummaries.Should().HaveCount(2).And.BeInAscendingOrder(x => x.RoundFinishedAt);
        readModel.Scores.Should().HaveCount(3);
        readModel.Scores.Should().BeInDescendingOrder(x => x.Points);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == storyTellerId);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == guessingPlayer1);
        readModel.Scores.Should().ContainSingle(x => x.Player.PlayerId == guessingPlayer2);
        readModel.IsNextGameRoomLinked.Should().BeFalse();
        readModel.NextGameRoomId.Should().Be(Guid.Empty);
    }

    [Fact]
    public async Task ReturnsIfNextGameRoomIsCreatedItsId()
    {
        var playerId = new PlayerId("thanos");
        var nextGameRoomId = new GameRoomId(Guid.NewGuid());
        var gameRoom = GameRoomBuilder.CreateFinished(GameRoomId, playerId, ["ironman", "gamora"])
            .WithNextGameRoomId(nextGameRoomId)
            .Build();
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

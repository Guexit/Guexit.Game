using System.Net.Http.Json;
using FluentAssertions.Execution;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameRoomVoting : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingGameRoomVoting(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsVotingReadModel()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var storyTellerUsername = "antman@guexit.com";
        var story = "El tipico adolescente abuelo";
        var storyTeller = new PlayerBuilder().WithId(storyTellerId).WithUsername(storyTellerUsername).Build();
        var guessingPlayer1 = new PlayerBuilder().WithId("player2").WithUsername("spiderman@guexit.com").Build();
        var guessingPlayer2 = new PlayerBuilder().WithId("player3").WithUsername("fury@guexit.com").Build();
        
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTeller.Id, [guessingPlayer1.Id, guessingPlayer2.Id])
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1.Id, guessingPlayer2.Id)
            .Build();
        
        await SaveInRepository(gameRoom);
        await SaveInRepository(storyTeller, guessingPlayer1, guessingPlayer2);
        
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId);
        var guessingPlayer1Card = gameRoom.SubmittedCards.First(x => x.PlayerId == guessingPlayer1.Id);
        var guessingPlayer2Card = gameRoom.SubmittedCards.First(x => x.PlayerId == guessingPlayer2.Id);

        using var voteResponse = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{GameRoomId.Value}/submitted-cards/{storyTellerCard.Card.Id.Value}/vote", 
            authenticatedPlayerId: guessingPlayer1.Id
        );
        await voteResponse.ShouldHaveSuccessStatusCode();

        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/voting", authenticatedPlayerId: storyTellerId);
        await response.ShouldHaveSuccessStatusCode();

        var votingReadModel = await response.Content.ReadFromJsonAsync<VotingReadModel>();
        
        votingReadModel.Should().NotBeNull();
        votingReadModel!.IsCurrentUserStoryTeller.Should().BeTrue();
        votingReadModel.CurrentStoryTeller.PlayerId.Should().Be(storyTellerId.Value);
        votingReadModel.CurrentStoryTeller.Username.Should().Be(storyTellerUsername);
        votingReadModel.CurrentStoryTeller.Story.Should().Be(story);
        votingReadModel.CurrentUserHasAlreadyVoted.Should().BeFalse();
        votingReadModel.Cards.Should().HaveCount(3);
        votingReadModel.Cards.Should()
            .Contain(x => x.Id == storyTellerCard.Card.Id && x.Url == storyTellerCard.Card.Url && x.WasSubmittedByQueryingPlayer)
            .And.Contain(x => x.Id == guessingPlayer1Card.Card.Id && x.Url == guessingPlayer1Card.Card.Url && !x.WasSubmittedByQueryingPlayer)
            .And.Contain(x => x.Id == guessingPlayer2Card.Card.Id && x.Url == guessingPlayer2Card.Card.Url && !x.WasSubmittedByQueryingPlayer);

        votingReadModel.CurrentUserVotedCard.Should().BeNull();
        
        votingReadModel.GuessingPlayers.Should().HaveCount(2);
        
        var spiderMan = votingReadModel.GuessingPlayers.Single(x => x.PlayerId == guessingPlayer1.Id);
        spiderMan.HasVotedAlready.Should().BeTrue();
        spiderMan.Username.Should().Be(guessingPlayer1.Username);
        spiderMan.Nickname.Should().Be(guessingPlayer1.Nickname.Value);
        
        var fury = votingReadModel.GuessingPlayers.Single(x => x.PlayerId == guessingPlayer2.Id);
        fury.HasVotedAlready.Should().BeFalse();
        fury.Username.Should().Be(guessingPlayer2.Username);
        fury.Nickname.Should().Be(guessingPlayer2.Nickname.Value);
    }

    [Fact]
    public async Task ReturnsVotedCardFromVotingPlayer()
    {
        var votingPlayerId = new PlayerId("player2");
        var storyTeller = new PlayerBuilder().WithId("storyTellerId").WithUsername("antman@guexit.com").Build();
        var guessingPlayer1 = new PlayerBuilder().WithId(votingPlayerId).WithUsername("spiderman@guexit.com").Build();
        var guessingPlayer2 = new PlayerBuilder().WithId("player3").WithUsername("fury@guexit.com").Build();
        
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTeller.Id, [guessingPlayer1.Id, guessingPlayer2.Id])
            .WithStoryTellerStory("El tipico adolescente abuelo")
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1.Id, guessingPlayer2.Id)
            .WithVote(votingPlayerId, storyTeller.Id)
            .Build();

        await SaveInRepository(gameRoom);
        await SaveInRepository(storyTeller, guessingPlayer1, guessingPlayer2);
        
        var cardVoted = gameRoom.SubmittedCards.First(x => x.Voters.Contains(votingPlayerId));
        using var response = await Send(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/voting", votingPlayerId);
        await response.ShouldHaveSuccessStatusCode();
        
        var votingReadModel = await response.Content.ReadFromJsonAsync<VotingReadModel>();
        votingReadModel.Should().NotBeNull();
        votingReadModel!.CurrentUserVotedCard.Should().NotBeNull();
        votingReadModel.CurrentUserVotedCard!.Id.Should().Be(cardVoted.Card.Id);
        votingReadModel.CurrentUserVotedCard.Url.Should().Be(cardVoted.Card.Url);
    }
}

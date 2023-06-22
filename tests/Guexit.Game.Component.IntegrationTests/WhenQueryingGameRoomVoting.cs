using System.Net.Http.Json;
using FluentAssertions.Execution;
using Guexit.Game.Component.IntegrationTests.Builders;
using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenQueryingGameRoomVoting : ComponentTest
{
    private static readonly GameRoomId GameRoomId = new(Guid.NewGuid());

    public WhenQueryingGameRoomVoting(GameWebApplicationFactory factory) : base(factory)
    { }

    [Fact]
    public async Task ReturnsGameBoardReadModel()
    {
        var storyTellerId = new PlayerId("storyTellerId");
        var storyTellerUsername = "antman";
        var story = "El tipico adolescente abuelo";
        var guessingPlayer1 = new PlayerId("player2");
        var guessingPlayer2 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { guessingPlayer1, guessingPlayer2 })
            .WithStoryTellerStory(story)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1, guessingPlayer2)
            .Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(storyTellerId).WithUsername(storyTellerUsername).Build(),
            new PlayerBuilder().WithId(guessingPlayer1).WithUsername("spiderman").Build(),
            new PlayerBuilder().WithId(guessingPlayer2).WithUsername("fury").Build()
        });
        var storyTellerCard = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId);
        var guessingPlayer1Card = gameRoom.SubmittedCards.First(x => x.PlayerId == guessingPlayer1);
        var guessingPlayer2Card = gameRoom.SubmittedCards.First(x => x.PlayerId == guessingPlayer2);
        await VoteCard(guessingPlayer1, storyTellerCard.Card.Id);

        var response = await GetGameRoomVoting(storyTellerId, gameRoom);

        using var _ = new AssertionScope();
        await response.ShouldHaveSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<VotingReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.IsCurrentUserStoryTeller.Should().BeTrue();
        responseContent.CurrentUserHasAlreadyVoted.Should().BeFalse();
        responseContent.Cards.Should().HaveCount(3);
        responseContent.PlayersWhoHaveAlreadyVoted.Should().HaveCount(1);
        responseContent.CurrentStoryTeller.PlayerId.Should().Be(storyTellerId.Value);
        responseContent.CurrentStoryTeller.Username.Should().Be(storyTellerUsername);
        responseContent.CurrentStoryTeller.Story.Should().Be(story);

        responseContent.Cards.Should()
            .Contain(x => x.Id == storyTellerCard.Card.Id && x.Url == storyTellerCard.Card.Url && x.WasSubmittedByQueryingPlayer)
            .And.Contain(x => x.Id == guessingPlayer1Card.Card.Id && x.Url == guessingPlayer1Card.Card.Url && !x.WasSubmittedByQueryingPlayer)
            .And.Contain(x => x.Id == guessingPlayer2Card.Card.Id && x.Url == guessingPlayer2Card.Card.Url && !x.WasSubmittedByQueryingPlayer);
        responseContent.PlayersWhoHaveAlreadyVoted.Should()
            .Contain(x => x.Username == "spiderman" && x.PlayerId == guessingPlayer1);
    }

    private async Task VoteCard(PlayerId votingPlayerId, CardId votedCardId)
    {
        using var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"/game-rooms/{GameRoomId.Value}/submitted-cards/{votedCardId.Value}/vote");
        request.AddPlayerIdHeader(votingPlayerId);
        var response =  await client.SendAsync(request);
        await response.ShouldHaveSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> GetGameRoomVoting(PlayerId storyTellerId, GameRoom gameRoom)
    {
        var client = WebApplicationFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/game-rooms/{gameRoom.Id.Value}/voting");
        request.AddPlayerIdHeader(storyTellerId);
        var response =  await client.SendAsync(request);

        await response.ShouldHaveSuccessStatusCode();
        return response;
    }
}

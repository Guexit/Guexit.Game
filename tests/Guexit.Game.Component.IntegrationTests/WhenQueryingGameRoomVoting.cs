using System.Net.Http.Json;
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
        var guessingPlayer1 = new PlayerId("player2");
        var guessingPlayer2 = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(GameRoomId, storyTellerId, new[] { guessingPlayer1, guessingPlayer2 })
            .WithStoryTellerStory("El tipico adolescente abuelo")
            .WithGuessingPlayerThatSubmittedCard(guessingPlayer1, guessingPlayer2)
            .Build();
        await Save(gameRoom);
        await Save(new[]
        {
            new PlayerBuilder().WithId(storyTellerId).WithUsername("antman").Build(),
            new PlayerBuilder().WithId(guessingPlayer1).WithUsername("spiderman").Build(),
            new PlayerBuilder().WithId(guessingPlayer2).WithUsername("fury").Build()
        });
        var votedCardId1 = gameRoom.SubmittedCards.First(x => x.PlayerId == storyTellerId).Card.Id;
        var votedCardId2 = gameRoom.SubmittedCards.First(x => x.PlayerId == guessingPlayer1).Card.Id;
        await VoteCard(guessingPlayer1, votedCardId1);
        await VoteCard(guessingPlayer2, votedCardId2);

        var response = await GetGameRoomVoting(storyTellerId, gameRoom);

        var responseContent = await response.Content.ReadFromJsonAsync<VotingReadModel>();
        responseContent.Should().NotBeNull();
        responseContent!.Cards.Should().HaveCount(3);

        responseContent.Cards.Single(x => x.Id == votedCardId1).Voters.Should().HaveCount(1);
        responseContent.Cards.Single(x => x.Id == votedCardId1).Voters.Should().Contain(guessingPlayer1.Value);

        responseContent.Cards.Single(x => x.Id == votedCardId2).Voters.Should().HaveCount(1);
        responseContent.Cards.Single(x => x.Id == votedCardId2).Voters.Should().Contain(guessingPlayer2.Value);
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

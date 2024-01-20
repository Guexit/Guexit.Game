using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenVotingSubmittedCard : ComponentTest
{
    public WhenVotingSubmittedCard(GameWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task VoteIsSubmitted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var votingPlayerId = new PlayerId("player2");
        var otherGuessingPlayerId = new PlayerId("player3");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, "storyTellerId", [votingPlayerId, otherGuessingPlayerId])
            .WithStoryTellerStory("Any card story")
            .WithGuessingPlayerThatSubmittedCard(votingPlayerId, otherGuessingPlayerId)
            .Build();
        var votedCard = gameRoom.SubmittedCards.First(x => x.PlayerId != votingPlayerId).Card;
        await Save(gameRoom);

        using var response = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoomId.Value}/submitted-cards/{votedCard.Id.Value}/vote", 
            votingPlayerId
        );

        await response.ShouldHaveSuccessStatusCode();
        await AssertCardIsVotedByGuessingPlayer(gameRoomId, votingPlayerId, votedCard.Id);
    }

    private async Task AssertCardIsVotedByGuessingPlayer(GameRoomId gameRoomId, PlayerId votingPlayerId, CardId votedCardId)
    {
        await using var scope = WebApplicationFactory.Services.CreateAsyncScope();
        var gameRoomRepository = scope.ServiceProvider.GetRequiredService<IGameRoomRepository>();
        var gameRoom = await gameRoomRepository.GetBy(gameRoomId);

        var submittedCard = gameRoom!.SubmittedCards.Single(x => x.Card.Id == votedCardId);
        submittedCard.Voters.Should().Contain(votingPlayerId);
    }
}
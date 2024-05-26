using Guexit.Game.Component.IntegrationTests.Extensions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Tests.Common.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Guexit.Game.Component.IntegrationTests;

public sealed class WhenVotingSubmittedCard : ComponentTest
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhenVotingSubmittedCard(GameWebApplicationFactory factory) : base(factory)
    {
        _serviceScopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();
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
        await SaveInRepository(gameRoom);

        using var response = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoomId.Value}/submitted-cards/{votedCard.Id.Value}/vote", 
            votingPlayerId
        );

        await response.ShouldHaveSuccessStatusCode();
        await AssertCardIsVotedByGuessingPlayer(gameRoomId, votingPlayerId, votedCard.Id);
    }
    
    [Fact]
    public async Task CardsAreReturnedToAvailablePoolAfterCompletingCurrentRound()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var player1 = new PlayerId("thanos");
        var player2 = new PlayerId("spiderman");
        var playerPendingToVote = new PlayerId("ironman");
        var gameRoom = GameRoomBuilder.CreateStarted(gameRoomId, player1, [player2, playerPendingToVote])
            .WithStoryTellerStory("Any Story")
            .WithGuessingPlayerThatSubmittedCard(player2, playerPendingToVote)
            .WithVote(votingPlayer: player2, cardSubmittedBy: player1)
            .WithPlayerThatReservedCardsForReRoll(player2)
            .WithPlayerThatReservedCardsForReRoll(playerPendingToVote)
            .Build();
        await SaveInRepository(gameRoom);
        
        var notCompletedCardReRollUrls = gameRoom.CurrentCardReRolls.SelectMany(x => x.ReservedCards).Select(x => x.Url).ToArray(); 
        await SaveInRepository(notCompletedCardReRollUrls.Select(url => 
            new ImageBuilder().WithGameRoomId(gameRoomId).WithUrl(url).Build()).ToArray());
    
        var anyCardId = gameRoom.SubmittedCards.First(x => x.PlayerId != playerPendingToVote).Card.Id;
        
        using var response = await Send(
            HttpMethod.Post, 
            $"/game-rooms/{gameRoomId.Value}/submitted-cards/{anyCardId.Value}/vote", 
            playerPendingToVote
        );
        await response.ShouldHaveSuccessStatusCode();

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var imageRepository = scope.ServiceProvider.GetRequiredService<IImageRepository>();

        var images = await imageRepository.GetBy(notCompletedCardReRollUrls);
        images.Should().AllSatisfy(x =>
        {
            x.IsAssignedToAGameRoom.Should().BeFalse();
            x.GameRoomId.Should().Be(GameRoomId.Empty);
        });
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
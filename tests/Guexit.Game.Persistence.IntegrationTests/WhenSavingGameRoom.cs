using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Repositories;
using Guexit.Game.Tests.Common;
using Guexit.Game.Tests.Common.Builders;
using Xunit.Abstractions;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class WhenSavingGameRoom : DatabaseMappingIntegrationTest
{
    public WhenSavingGameRoom(IntegrationTestFixture fixture, ITestOutputHelper testOutput) : base(fixture, testOutput)
    { }

    [Fact]
    public async Task IsPersisted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("creatorPlayerId");
        var createdAt = new DateTimeOffset(2022, 1, 2, 3, 5, 6, TimeSpan.Zero);
        var repository = new GameRoomRepository(DbContext);
        var submittedStory = "Some story";
        var guessingPlayerIdsThatSubmittedCard = new PlayerId[] { "invitedPlayer1", "invitedPlayer2" };

        await repository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(initialStoryTeller)
            .WithPlayersThatJoined("invitedPlayer1", "invitedPlayer2", "invitedPlayer3")
            .WithCreatedAt(createdAt)
            .Started()
            .WithAssignedDeck(Enumerable.Range(0, 100).Select(x => new CardBuilder().WithUrl(new Uri($"https://pablocompany/{x}"))).ToArray())
            .WithStoryTellerStory(submittedStory)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayerIdsThatSubmittedCard)
            .WithPlayerThatReservedCardsForReRoll(initialStoryTeller)
            .Build());
        await SaveChanges();

        var gameRoom = await repository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Id.Should().Be(gameRoomId);
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.PlayerIds.Should().BeEquivalentTo(new PlayerId[] { initialStoryTeller, "invitedPlayer1", "invitedPlayer2", "invitedPlayer3" });
        gameRoom.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().NotBeEmpty();
        gameRoom.PlayerHands.Should().NotBeEmpty();
        gameRoom.CurrentStoryTeller.PlayerId.Should().Be(initialStoryTeller);
        gameRoom.CurrentStoryTeller.Story.Should().Be(submittedStory);
        gameRoom.SubmittedCards.Should().HaveCount(3);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(x => x == initialStoryTeller);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(guessingPlayerIdsThatSubmittedCard[0]);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(guessingPlayerIdsThatSubmittedCard[1]);
        gameRoom.NextGameRoomId.Should().Be(GameRoomId.Empty);
        gameRoom.CurrentCardReRolls.Should().HaveCount(1);
        gameRoom.CurrentCardReRolls.Single().PlayerId.Should().Be(initialStoryTeller);
        gameRoom.CurrentCardReRolls.Single().Status.Should().Be(CardReRollStatus.InProgress);
        gameRoom.CurrentCardReRolls.Single().ReservedCards.Should().HaveCount(CardReRoll.RequiredReservedCardsSize);
    }
}
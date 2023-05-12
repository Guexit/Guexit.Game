using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Repositories;
using Guexit.Game.Tests.Common;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class WhenSavingGameRoom : DatabaseMappingIntegrationTest
{
    public WhenSavingGameRoom(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task IsPersisted()
    {
        var gameRoomId = new GameRoomId(Guid.NewGuid());
        var initialStoryTeller = new PlayerId("creatorPlayerId");
        var createdAt = new DateTimeOffset(2022, 1, 2, 3, 5, 6, TimeSpan.Zero);
        var repository = new GameRoomRepository(DbContext);
        var requiredMinPlayers = 4;
        var submittedStory = "Some story";
        var guessingPlayerdIdsThatSubmittedCard = new PlayerId[] { "invitedPlayer1", "invitedPlayer2" };

        await repository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(initialStoryTeller)
            .WithPlayersThatJoined("invitedPlayer1", "invitedPlayer2", "invitedPlayer3")
            .WithCreatedAt(createdAt)
            .WithMinRequiredPlayers(requiredMinPlayers)
            .Started()
            .WithDeck(Enumerable.Range(0, 100).Select(x => new CardBuilder().WithUrl(new Uri($"https://pablocompany/{x}"))).ToArray())
            .WithStoryTellerStory(submittedStory)
            .WithGuessingPlayerThatSubmittedCard(guessingPlayerdIdsThatSubmittedCard)
            .Build());
        await SaveChangesAndClearChangeTracking();

        var gameRoom = await repository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Id.Should().Be(gameRoomId);
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.PlayerIds.Should().BeEquivalentTo(new PlayerId[] { initialStoryTeller, "invitedPlayer1", "invitedPlayer2", "invitedPlayer3" });
        gameRoom.RequiredMinPlayers.Count.Should().Be(requiredMinPlayers);
        gameRoom.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().NotBeEmpty();
        gameRoom.PlayerHands.Should().NotBeEmpty();
        gameRoom.CurrentStoryTeller.PlayerId.Should().Be(initialStoryTeller);
        gameRoom.CurrentStoryTeller.Story.Should().Be(submittedStory);
        gameRoom.SubmittedCards.Should().HaveCount(3);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(x => x == initialStoryTeller);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(guessingPlayerdIdsThatSubmittedCard[0]);
        gameRoom.SubmittedCards.Select(x => x.PlayerId).Should().Contain(guessingPlayerdIdsThatSubmittedCard[1]);
    }
}
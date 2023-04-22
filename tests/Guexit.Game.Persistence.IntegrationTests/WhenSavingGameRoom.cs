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
        var creatorId = new PlayerId(Guid.NewGuid().ToString());
        var createdAt = new DateTimeOffset(2022, 1, 2, 3, 5, 6, TimeSpan.Zero);
        var repository = new GameRoomRepository(DbContext);
        var requiredMinPlayers = 4;

        await repository.Add(new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creatorId)
            .WithPlayersThatJoined("1", "2", "3")
            .WithCreatedAt(createdAt)
            .WithMinRequiredPlayers(requiredMinPlayers)
            .WithDeck(Enumerable.Range(0, 100).Select(x => new CardBuilder().WithUrl(new Uri($"https://pablocompany/{x}"))).ToArray())
            .Build());
        await SaveChangesAndClearChangeTracking();

        var gameRoom = await repository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Id.Should().Be(gameRoomId);
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.PlayerIds.Should().BeEquivalentTo(new[] { creatorId, new PlayerId("1"), new PlayerId("2"), new PlayerId("3") });
        gameRoom.RequiredMinPlayers.Count.Should().Be(requiredMinPlayers);
        gameRoom.Status.Should().Be(GameStatus.InProgress);
        gameRoom.Deck.Should().NotBeEmpty();
        gameRoom.PlayerHands.Should().NotBeEmpty();
        gameRoom.CurrentStoryTeller.Should().Be(StoryTeller.Empty);
    }
}
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;
using TryGuessIt.Game.Persistence.Repositories;

namespace TryGuessIt.Game.Persistence.IntegrationTests;

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

        await repository.Add(new GameRoom(gameRoomId, creatorId, createdAt));
        await SaveChangesAndClearChangeTracking();

        var gameRoom = await repository.GetBy(gameRoomId);
        gameRoom.Should().NotBeNull();
        gameRoom!.Id.Should().Be(gameRoomId);
        gameRoom.CreatedAt.Should().Be(createdAt);
        gameRoom.PlayerIds.Should().BeEquivalentTo(new[] { creatorId });
    }
}
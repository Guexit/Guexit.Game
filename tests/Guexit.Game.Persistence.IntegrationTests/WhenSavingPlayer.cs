using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TryGuessIt.Game.Persistence.Repositories;

namespace TryGuessIt.Game.Persistence.IntegrationTests;

public sealed class WhenSavingPlayer : DatabaseMappingIntegrationTest
{
    public WhenSavingPlayer(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task IsPersisted()
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        var username = Guid.NewGuid().ToString();
        var repository = new PlayerRepository(DbContext);

        await repository.Add(new Player(playerId, username));
        await SaveChangesAndClearChangeTracking();

        var player = await repository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
        player.Username.Should().Be(username);
    }
}

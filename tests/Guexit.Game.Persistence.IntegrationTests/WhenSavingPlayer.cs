using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Repositories;
using Xunit.Abstractions;

namespace Guexit.Game.Persistence.IntegrationTests;

public sealed class WhenSavingPlayer : DatabaseMappingIntegrationTest
{
    public WhenSavingPlayer(IntegrationTestFixture fixture, ITestOutputHelper testOutput) : base(fixture, testOutput)
    {
    }

    [Fact]
    public async Task IsPersisted()
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        var username = Guid.NewGuid().ToString();
        var repository = new PlayerRepository(DbContext);

        await repository.Add(new Player(playerId, username));
        await SaveChanges();

        var player = await repository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
        player.Username.Should().Be(username);
    }
}

using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenCreatingPlayer
{
    private readonly IPlayerRepository _playerRepository;
    private readonly PlayerManagementService _service;

    public WhenCreatingPlayer()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _service = new PlayerManagementService(_playerRepository);
    }

    [Fact]
    public async Task IsCreated()
    {
        var playerId = new PlayerId(Guid.NewGuid().ToString());
        var username = "batman";

        await _service.CreatePlayer(playerId, username);

        var player = await _playerRepository.GetBy(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
        player.Username.Should().Be(username);
    }
}

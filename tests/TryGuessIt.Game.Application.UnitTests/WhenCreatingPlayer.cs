using FluentAssertions;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public class WhenCreatingPlayer
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerManagementService _playerManagementService;

    public WhenCreatingPlayer()
    {
        _playerRepository = new FakeInMemoryPlayerRepository();
        _playerManagementService = new PlayerManagementService(_playerRepository);
    }

    [Fact]
    public async Task IsCreated()
    {
        var playerId = Guid.NewGuid().ToString();

        await _playerManagementService.CreatePlayer(playerId);

        var player = await _playerRepository.GetById(playerId);
        player.Should().NotBeNull();
        player!.Id.Should().Be(playerId);
    }
}

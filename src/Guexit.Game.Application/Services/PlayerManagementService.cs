using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Services;

public interface IPlayerManagementService
{
    Task CreatePlayer(PlayerId playerId, string username, CancellationToken cancellationToken = default);
}

public sealed class PlayerManagementService : IPlayerManagementService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerManagementService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task CreatePlayer(PlayerId playerId, string username, CancellationToken cancellationToken = default)
    {
        var existingPlayer = await _playerRepository.GetBy(playerId, cancellationToken);
        if (existingPlayer is not null)
            return;

        await _playerRepository.Add(new Player(playerId, username), cancellationToken);
    }
}

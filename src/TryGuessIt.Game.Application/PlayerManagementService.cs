using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application;

public interface IPlayerManagementService
{
    Task CreatePlayer(string playerId);
}

public sealed class PlayerManagementService : IPlayerManagementService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerManagementService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task CreatePlayer(string playerId)
    {
        await _playerRepository.Add(new Player(playerId));
    }
}

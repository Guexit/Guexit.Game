using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class FakeInMemoryPlayerRepository : IPlayerRepository
{
    private readonly Dictionary<PlayerId, Player> _players = new();

    public Task Add(Player player, CancellationToken cancellationToken = default)
    {
        _players.Add(player.Id, player);
        return Task.CompletedTask;
    }

    public Task<Player?> GetBy(PlayerId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_players.GetValueOrDefault(id));
    }
}

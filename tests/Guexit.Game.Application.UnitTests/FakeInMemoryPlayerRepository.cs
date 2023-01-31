using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class FakeInMemoryPlayerRepository : IPlayerRepository
{
    private readonly List<Player> _players = new();

    public Task Add(Player player, CancellationToken cancellationToken = default)
    {
        _players.Add(player);
        return Task.CompletedTask;
    }

    public Task<Player?> GetBy(PlayerId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_players.FirstOrDefault(x => x.Id == id));
    }
}

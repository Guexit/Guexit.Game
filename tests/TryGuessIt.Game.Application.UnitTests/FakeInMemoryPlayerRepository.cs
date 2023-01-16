using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.UnitTests;

public sealed class FakeInMemoryPlayerRepository : IPlayerRepository
{
    private readonly List<Player> _players = new();

    public Task Add(Player player, CancellationToken cancellationToken = default)
    {
        _players.Add(player);
        return Task.CompletedTask;
    }

    public Task<Player?> GetBy(PlayerId playerId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_players.FirstOrDefault(x => x.Id == playerId));
    }
}

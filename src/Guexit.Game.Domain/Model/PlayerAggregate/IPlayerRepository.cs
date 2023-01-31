namespace Guexit.Game.Domain.Model.PlayerAggregate;

public interface IPlayerRepository
{
    Task Add(Player player, CancellationToken cancellationToken = default);
    Task<Player?> GetBy(PlayerId id, CancellationToken cancellationToken = default);
}

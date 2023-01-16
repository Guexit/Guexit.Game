namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public interface IPlayerRepository
{
    Task Add(Player player, CancellationToken cancellationToken = default);
    Task<Player?> GetBy(PlayerId playerId, CancellationToken cancellationToken = default);
}

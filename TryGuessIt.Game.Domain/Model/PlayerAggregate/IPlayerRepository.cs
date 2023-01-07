namespace TryGuessIt.Game.Domain.Model.PlayerAggregate;

public interface IPlayerRepository
{
    Task Add(Player player);
    Task<Player?> GetById(string playerId);
}

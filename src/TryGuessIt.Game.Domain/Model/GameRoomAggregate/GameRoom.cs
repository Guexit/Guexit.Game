using TryGuessIt.Game.Domain.Exceptions;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate.Events;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : Entity<GameRoomId>, IAggregateRoot
{
    public ICollection<PlayerId> PlayerIds { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;

    public GameRoom() : base()
    {
        // Entity Framework required parameterless ctor
    }

    public GameRoom(GameRoomId id, PlayerId creatorId, DateTimeOffset createdAt) 
        : base(id)
    {
        CreatedAt = createdAt;
        PlayerIds = new List<PlayerId>() { creatorId };
    }

    public void Join(PlayerId playerId)
    {
        if (PlayerIds.Contains(playerId))
            throw new PlayerIsAlreadyInGameRoomException(playerId);

        PlayerIds.Add(playerId);

        AddDomainEvent(new PlayerJoinedGameRoom(Id, playerId));
    }
}

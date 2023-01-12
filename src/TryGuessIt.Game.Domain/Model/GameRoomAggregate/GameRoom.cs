﻿using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : Entity<GameRoomId>, IAggregateRoot
{
    public ICollection<PlayerId> PlayerIds { get; private set; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }

    public GameRoom(GameRoomId id) : base(id)
    {

    }

    public GameRoom(GameRoomId id, PlayerId creatorId, DateTimeOffset createdAt) 
        : base(id)
    {
        CreatedAt = createdAt;
        PlayerIds = new HashSet<PlayerId>() { creatorId };
    }
}

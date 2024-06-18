using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoomPlayer : ValueObject
{
    public GameRoomId GameRoomId { get; private init; }
    public PlayerId PlayerId { get; private init; }

    public GameRoomPlayer()
    {
        // Entity Framework required parameterless ctor
    }
    
    public GameRoomPlayer(GameRoomId gameRoomId, PlayerId playerId)
    {
        GameRoomId = gameRoomId;
        PlayerId = playerId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return GameRoomId;
        yield return PlayerId;
    }
}
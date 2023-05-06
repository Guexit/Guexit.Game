using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class PlayerHand : Entity<PlayerHandId>
{
    public PlayerId PlayerId { get; private set; } = default!;
    public List<Card> Cards { get; private set; } = new();
    public GameRoomId GameRoomId { get; private set; } = default!;

    private PlayerHand()
    {
        // Entity Framework required parameterless ctor 
    }

    public PlayerHand(PlayerHandId id, PlayerId playerId, List<Card> cards, GameRoomId gameRoomId)
    {
        Id = id;
        PlayerId = playerId;
        Cards = cards;
        GameRoomId = gameRoomId;
    }
}

public sealed class PlayerHandId : ValueObject
{
    public static readonly PlayerHandId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public PlayerHandId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator PlayerHandId(Guid value) => new(value);
    public static implicit operator Guid(PlayerHandId value) => value.Value;
}

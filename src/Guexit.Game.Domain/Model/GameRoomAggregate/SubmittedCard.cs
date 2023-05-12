using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class SubmittedCard : Entity<SubmittedCardId>
{
    public PlayerId PlayerId { get; private set; } = default!;
    public Card Card { get; private set; } = default!;
    public GameRoomId GameRoomId { get; private set; } = default!;

    private SubmittedCard()
    {
        // Entity Framework required parameterless ctor 
    }

    public SubmittedCard(PlayerId playerId, Card cards, GameRoomId gameRoomId)
    {
        Id = new SubmittedCardId(Guid.NewGuid());
        PlayerId = playerId;
        Card = cards;
        GameRoomId = gameRoomId;
    }
}

public sealed class SubmittedCardId : ValueObject
{
    public static readonly SubmittedCardId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public SubmittedCardId(Guid value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator SubmittedCardId(Guid value) => new(value);
    public static implicit operator Guid(SubmittedCardId value) => value.Value;
}
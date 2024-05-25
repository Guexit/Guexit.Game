using System.Collections.Frozen;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class CardReRoll : Entity<CardReRollId>
{
    public const int RequiredReservedCardsSize = 3;

    public PlayerId PlayerId { get; private init; } = null!;
    public ICollection<Card> ReservedCards { get; private init; } = new List<Card>();
    public bool IsCompleted { get; private set; }

    private CardReRoll()
    {
        // Entity Framework required parameterless ctor
    }

    public CardReRoll(CardReRollId id, PlayerId playerId, Card[] cards)
    {
        Id = id;
        PlayerId = playerId;
        ReservedCards = cards;
        IsCompleted = false;
    }
}

public sealed class CardReRollId : ValueObject
{
    public static readonly CardReRollId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public CardReRollId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

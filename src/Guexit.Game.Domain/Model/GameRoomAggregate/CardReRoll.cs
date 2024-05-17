using System.Collections.Frozen;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class CardReRoll : Entity<CardReRollId>
{
    public const int RequiredReservedCardsSize = 3;

    public PlayerId PlayerId { get; private init; } = null!;
    public ICollection<Card> ReservedCards { get; private init; } = new List<Card>();
    public CardReRollStatus Status { get; private init; } = default!;

    private CardReRoll()
    {
        // Entity Framework required parameterless ctor
    }

    public CardReRoll(CardReRollId id, PlayerId playerId, Card[] cards)
    {
        Id = id;
        PlayerId = playerId;
        ReservedCards = cards;
        Status = CardReRollStatus.InProgress;
    }
}

public sealed class CardReRollStatus : ValueObject
{
    public static readonly CardReRollStatus Cancelled = new("Cancelled");
    public static readonly CardReRollStatus InProgress = new("InProgress");
    public static readonly CardReRollStatus Completed = new("Completed");
    
    private static readonly FrozenDictionary<string, CardReRollStatus> AllStatusByValue = FrozenDictionary.ToFrozenDictionary(new[]
    {
        KeyValuePair.Create(Cancelled.Value, Cancelled),
        KeyValuePair.Create(InProgress.Value, InProgress),
        KeyValuePair.Create(Completed.Value, Completed)
    });

    public string Value { get; }

    private CardReRollStatus(string value)
    {
        Value = value;
    }

    public static CardReRollStatus From(string value)
    {
        if (AllStatusByValue.TryGetValue(value, out var status))
            return status;

        throw new ArgumentOutOfRangeException(nameof(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
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

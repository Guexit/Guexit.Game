namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class CardId : ValueObject
{
    internal static readonly CardId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public CardId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator CardId(Guid value) => new(value);
    public static implicit operator Guid(CardId value) => value.Value;
}

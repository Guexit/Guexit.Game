namespace Guexit.Game.Domain.Model.GameRoomAggregate;

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

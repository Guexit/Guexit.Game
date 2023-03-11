namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoomId : ValueObject
{
    internal static readonly GameRoomId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public GameRoomId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator GameRoomId(Guid value) => new(value);
    public static implicit operator Guid(GameRoomId value) => value.Value;
}

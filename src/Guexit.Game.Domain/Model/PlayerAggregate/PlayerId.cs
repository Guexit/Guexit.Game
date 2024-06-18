namespace Guexit.Game.Domain.Model.PlayerAggregate;

public class PlayerId : ValueObject
{
    internal static readonly PlayerId Empty = new("Empty");

    public string Value { get; private init; } = null!;

    public PlayerId()
    {
        // EF required parameterless ctor
    }
    
    public PlayerId(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator PlayerId(string value) => new(value);
    public static implicit operator string(PlayerId value) => value.Value;
}

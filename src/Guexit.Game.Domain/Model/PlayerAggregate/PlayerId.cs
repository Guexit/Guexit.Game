namespace Guexit.Game.Domain.Model.PlayerAggregate;

public class PlayerId : ValueObject
{
    public string Value { get; }

    public PlayerId(string value)
    {
        if (string.IsNullOrEmpty(value)) 
            throw new ArgumentNullException(nameof(value));

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

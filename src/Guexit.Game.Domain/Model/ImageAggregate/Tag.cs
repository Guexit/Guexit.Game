namespace Guexit.Game.Domain.Model.ImageAggregate;

public sealed class Tag : ValueObject
{
    public string Value { get; init; } = null!;
    
    private Tag() { /* Entity Framework required parameterless ctor */ }
    
    public Tag(string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        
        Value = tag;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
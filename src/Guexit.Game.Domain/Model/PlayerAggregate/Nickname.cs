using System.Collections.Frozen;
using System.Text;

namespace Guexit.Game.Domain.Model.PlayerAggregate;

public sealed class Nickname : ValueObject
{
    private static readonly FrozenSet<char> CharactersToTrim = new[] { '.', '_', '-', '+' }.ToFrozenSet();
    
    public string Value { get; }

    public Nickname(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        Value = value;
    }

    public static Nickname From(string username)
    {
        var span = username.AsSpan();

        var atIndex = span.IndexOf('@');
        if (atIndex is not -1) 
            span = span[..atIndex];
        
        var sb = new StringBuilder();
        foreach (var character in span)
        {
            if (CharactersToTrim.Contains(character))
                continue;

            sb.Append(character);
        }
        
        return sb.Length > 0 ? new Nickname(sb.ToString()) : new Nickname(username);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
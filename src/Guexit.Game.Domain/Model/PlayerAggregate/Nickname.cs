using System.Collections.Frozen;
using System.Text;
using Guexit.Game.Domain.Exceptions;

namespace Guexit.Game.Domain.Model.PlayerAggregate;

public sealed class Nickname : ValueObject
{
    private static readonly FrozenSet<char> _charactersToTrim = new char[] { '.', '_', '-', '+' }.ToFrozenSet();

    public string Value { get; }

    public Nickname(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidNicknameException();
        
        Value = value;
    }

    public static Nickname From(string username)
    {
        var span = username.AsSpan();

        var atIndex = span.IndexOf('@');
        if (atIndex is not -1) 
            span = span[..atIndex];
        
        var sb = new StringBuilder(span.Length);
        foreach (var character in span)
        {
            if (_charactersToTrim.Contains(character))
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
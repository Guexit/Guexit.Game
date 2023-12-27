using System.Collections.Frozen;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameStatus : ValueObject
{
    public static readonly GameStatus NotStarted = new("NotStarted");
    public static readonly GameStatus InProgress = new("InProgress");
    public static readonly GameStatus Finished = new("Finished");
    private static readonly FrozenDictionary<string, GameStatus> AllStatusByValue = FrozenDictionary.ToFrozenDictionary(new[]
    {
        KeyValuePair.Create(NotStarted.Value, NotStarted), 
        KeyValuePair.Create(InProgress.Value, InProgress), 
        KeyValuePair.Create(Finished.Value, Finished)
    });

    public string Value { get; }

    private GameStatus(string value)
    {
        Value = value;
    }

    public static GameStatus From(string value)
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

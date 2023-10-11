namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameStatus : ValueObject
{
    public static readonly GameStatus NotStarted = new("NotStarted");
    public static readonly GameStatus InProgress = new("InProgress");
    public static readonly GameStatus Finished = new("Finished");
    public static readonly GameStatus[] All = new[] { NotStarted, InProgress, Finished };

    public string Value { get; }

    private GameStatus(string value)
    {
        Value = value;
    }

    public static GameStatus From(string value)
    {
        foreach (var status in All)
        {
            if (status.Value == value)
            {
                return status;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

using TryGuessIt.Game.Domain.Exceptions;

namespace TryGuessIt.Game.Domain.Model.GameRoomAggregate;

public sealed class RequiredMinPlayers : ValueObject
{
    public static readonly RequiredMinPlayers Default = new RequiredMinPlayers(3);
    
    public int Count { get; }

    public RequiredMinPlayers(int count)
    {
        if (count < 3 || count > 10)
            throw new InvalidRequiredMinPlayersException(count);

        Count = count;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Count;
    }
}

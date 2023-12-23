using Guexit.Game.Domain.Exceptions;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class RequiredMinPlayers : ValueObject
{
    public static readonly RequiredMinPlayers Default = new(3);
    
    public int Count { get; }

    public RequiredMinPlayers(int count)
    {
        if (count is < 3 or > 10)
            throw new InvalidRequiredMinPlayersException(count);

        Count = count;
    }

    public bool IsSatisfiedBy(int playersCount) => playersCount >= Count;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Count;
    }
}

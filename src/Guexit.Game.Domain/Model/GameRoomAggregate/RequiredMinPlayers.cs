using Guexit.Game.Domain.Exceptions;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class RequiredMinPlayers : ValueObject
{
    public static readonly RequiredMinPlayers Default = new(3);
    
    public int Count { get; }

    public RequiredMinPlayers(int count)
    {
        if (count < 3 || count > 10)
            throw new InvalidRequiredMinPlayersException(count);

        Count = count;
    }

    public bool AreEnoughPlayers(int playersCount) => playersCount >= Count;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Count;
    }
}

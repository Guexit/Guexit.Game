using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Guexit.Game.Persistence.Mappings.ValueComparers;

public sealed class PlayerIdsCommaSeparatedTextCollectionValueComparer : ValueComparer<ICollection<PlayerId>>
{
    public PlayerIdsCommaSeparatedTextCollectionValueComparer()
        : base(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray())
    {

    }
}
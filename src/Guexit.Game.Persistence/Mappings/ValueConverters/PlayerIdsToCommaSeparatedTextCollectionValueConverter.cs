using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Guexit.Game.Persistence.Mappings.ValueConverters;

public sealed class PlayerIdsToCommaSeparatedTextCollectionValueConverter : ValueConverter<ICollection<PlayerId>, string>
{
    public PlayerIdsToCommaSeparatedTextCollectionValueConverter()
        : base(v => ToProvider(v), from => From(from))
    {
    }

    private static string ToProvider(ICollection<PlayerId> playerIds)
    {
        var text = string.Join(',', playerIds.Select(x => x.Value));
        return text;
    }

    private static ICollection<PlayerId> From(string playerIds)
    {
        var ids = playerIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var playerIdsToReturn = new List<PlayerId>(ids.Length);
        foreach (var id in ids)
        {
            playerIdsToReturn.Add(new PlayerId(id));
        }
        return playerIdsToReturn;
    }
}
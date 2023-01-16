using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Persistence.Mappings;

internal sealed class GameRoomEntityConfiguration : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.Property(x => x.Id).HasConversion<GameRoomIdValueConverter>();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlayerIds)
            .HasConversion<PlayerIdsCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCollectionValueComparer());


        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RequiredMinPlayers).HasConversion<RequiredMinPlayersValueConverter>();
    }
    
    private sealed class GameRoomIdValueConverter : ValueConverter<GameRoomId, Guid>
    {
        public GameRoomIdValueConverter()
            : base(to => to.Value, from => new GameRoomId(from))
        {
        }
    }

    private sealed class PlayerIdsCollectionValueComparer : ValueComparer<ICollection<PlayerId>>
    {
        public PlayerIdsCollectionValueComparer()
            : base(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToArray())
        {

        }
    }

    private sealed class PlayerIdsCollectionValueConverter : ValueConverter<ICollection<PlayerId>, string>
    {
        public PlayerIdsCollectionValueConverter()
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
            var ids = playerIds.Split(',');
            var playerIdsToReturn = new List<PlayerId>(ids.Length);
            foreach (var id in ids)
            {
                playerIdsToReturn.Add(new PlayerId(id));
            }
            return playerIdsToReturn;
        }
    }
    private sealed class RequiredMinPlayersValueConverter : ValueConverter<RequiredMinPlayers, int>
    {
        public RequiredMinPlayersValueConverter()
            : base(v => ToProvider(v), from => From(from))
        {
        }

        private static int ToProvider(RequiredMinPlayers requiredMinPlayers)
        {
            return requiredMinPlayers.Count;
        }

        private static RequiredMinPlayers From(int value)
        {
            
            return new RequiredMinPlayers(value);
        }
    }
}


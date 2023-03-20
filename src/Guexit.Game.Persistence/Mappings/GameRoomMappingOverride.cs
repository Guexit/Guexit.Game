using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class GameRoomMappingOverride : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new GameRoomId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlayerIds)
            .HasConversion<PlayerIdsCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCollectionValueComparer());

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RequiredMinPlayers)
            .HasConversion(to => to.Count, from => new RequiredMinPlayers(from))
            .IsRequired();
        
        builder.Property(x => x.Status)
            .HasConversion(to => to.Value, from => GameStatus.From(from))
            .IsRequired();

        builder.Property<uint>("Version").IsRowVersion();
    }

    private sealed class PlayerIdsCollectionValueComparer : ValueComparer<ICollection<PlayerId>>
    {
        public PlayerIdsCollectionValueComparer()
            : base(
                (c1, c2) => c1!.SequenceEqual(c2!),
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
}


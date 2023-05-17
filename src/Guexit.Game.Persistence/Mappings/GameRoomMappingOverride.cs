using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Mappings.ValueComparers;
using Guexit.Game.Persistence.Mappings.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class GameRoomMappingOverride : IEntityTypeConfiguration<GameRoom>
{
    public void Configure(EntityTypeBuilder<GameRoom> builder)
    {
        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new GameRoomId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlayerIds)
            .HasConversion<PlayerIdsToCommaSeparatedTextCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCommaSeparatedTextCollectionValueComparer());

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RequiredMinPlayers)
            .HasConversion(to => to.Count, from => new RequiredMinPlayers(from))
            .IsRequired();

        builder.OwnsOne(x => x.CurrentStoryTeller, st =>
        {
            st.Property(x => x.PlayerId).HasConversion(to => to.Value, from => new PlayerId(from));
            st.Property(x => x.Story);
        });

        builder.Property(x => x.Status)
            .HasConversion(to => to.Value, from => GameStatus.From(from))
            .IsRequired();

        builder.HasMany(x => x.PlayerHands).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Deck).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.SubmittedCards).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.FinishedRounds).WithOne().OnDelete(DeleteBehavior.Cascade);

        builder.Property<uint>("Version").IsRowVersion();
    }
}

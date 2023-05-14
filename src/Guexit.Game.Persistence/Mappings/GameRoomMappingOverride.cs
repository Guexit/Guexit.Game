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

        builder.OwnsMany(x => x.SubmittedCards, ownedBuilder =>
        {
            ownedBuilder.ToTable("SubmittedCards");

            ownedBuilder.HasKey(x => x.Id);
            ownedBuilder.Property(x => x.Id).HasConversion(x => x.Value, x => new SubmittedCardId(x));
            ownedBuilder.Property(x => x.PlayerId).HasConversion(x => x.Value, x => new PlayerId(x));
            ownedBuilder.Property(x => x.GameRoomId).HasConversion(x => x.Value, x => new GameRoomId(x));

            ownedBuilder.Property(x => x.Voters)
                .HasConversion<PlayerIdsToCommaSeparatedTextCollectionValueConverter>()
                .Metadata
                .SetValueComparer(new PlayerIdsCommaSeparatedTextCollectionValueComparer());

            ownedBuilder.HasOne(x => x.Card);

            ownedBuilder.HasIndex(x => new { x.GameRoomId, x.PlayerId }).IsUnique();
        });
        
        builder.Property(x => x.Status)
            .HasConversion(to => to.Value, from => GameStatus.From(from))
            .IsRequired();

        builder.HasMany(x => x.PlayerHands);
        builder.HasMany(x => x.Deck);

        builder.Property<uint>("Version").IsRowVersion();
    }
}

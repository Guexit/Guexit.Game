using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Mappings.ValueComparers;
using Guexit.Game.Persistence.Mappings.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class SubmittedCardMappingOverride : IEntityTypeConfiguration<SubmittedCard>
{
    public void Configure(EntityTypeBuilder<SubmittedCard> builder)
    {
        builder.ToTable("SubmittedCards");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SubmittedCardId(x));
        builder.Property(x => x.PlayerId).HasConversion(x => x.Value, x => new PlayerId(x));
        builder.Property(x => x.GameRoomId).HasConversion(x => x.Value, x => new GameRoomId(x));

        builder.Property(x => x.Voters)
            .HasConversion<PlayerIdsToCommaSeparatedTextCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCommaSeparatedTextCollectionValueComparer());

        builder.HasOne(x => x.Card);

        builder.HasIndex(x => new { x.GameRoomId, x.PlayerId }).IsUnique();

        builder.Navigation(x => x.Card).AutoInclude();
    }
}

using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence.Mappings.ValueComparers;
using Guexit.Game.Persistence.Mappings.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class SubmittedCardSnapshotMappingOverride : IEntityTypeConfiguration<SubmittedCardSnapshot>
{
    public void Configure(EntityTypeBuilder<SubmittedCardSnapshot> builder)
    {
        builder.ToTable("SubmittedCardSnapshots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new SubmittedCardSnapshotId(x));
        builder.Property(x => x.PlayerId).HasConversion(x => x.Value, x => new PlayerId(x));
        builder.Property(x => x.FinishedRoundId).HasConversion(x => x.Value, x => new FinishedRoundId(x));

        builder.Property(x => x.Voters)
            .HasConversion<PlayerIdsToCommaSeparatedTextCollectionValueConverter>()
            .Metadata
            .SetValueComparer(new PlayerIdsCommaSeparatedTextCollectionValueComparer());

        builder.HasOne(x => x.Card);
    }
}

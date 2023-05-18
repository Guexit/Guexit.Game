using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class FinishedRoundMappingOverride : IEntityTypeConfiguration<FinishedRound>
{
    public void Configure(EntityTypeBuilder<FinishedRound> builder)
    {
        builder.ToTable("FinishedRounds");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new FinishedRoundId(x));
        builder.Property(x => x.GameRoomId).HasConversion(x => x.Value, x => new GameRoomId(x));

        builder.OwnsMany(fr => fr.Scores, b =>
        {
            b.ToTable("Scores");

            b.HasKey("Id");
            b.Property(x => x.FinishedRoundId).HasConversion(x => x.Value, x => new FinishedRoundId(x));
            b.Property(x => x.PlayerId).HasConversion(x => x.Value, x => new PlayerId(x));
            b.Property(x => x.Points).HasConversion(x => x.Value, x => new Points(x));
        });

        builder.HasMany(fr => fr.SubmittedCardSnapshots);
    }
}

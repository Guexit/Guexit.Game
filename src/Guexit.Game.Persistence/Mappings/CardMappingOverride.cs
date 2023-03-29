using Guexit.Game.Domain.Model.GameRoomAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class CardMappingOverride : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new CardId(x));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired();
    }
}


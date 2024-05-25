using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class CardReRollMappingOverride : IEntityTypeConfiguration<CardReRoll>
{
    public void Configure(EntityTypeBuilder<CardReRoll> builder)
    {
        builder.ToTable("CardReRolls");

        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new CardReRollId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlayerId).HasConversion(to => to.Value, from => new PlayerId(from));
        builder.Property(x => x.IsCompleted);
        builder.HasMany(x => x.ReservedCards).WithOne().HasForeignKey("CardReRollId").OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.ReservedCards).AutoInclude();
    }
}

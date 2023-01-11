using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Persistence.EntityConfigurations;

public sealed class PlayerEntityConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new PlayerId(from));

        const int maxEmailLength = 320;
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(maxEmailLength);
    }
}


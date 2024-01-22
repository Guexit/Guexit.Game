using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class PlayerMappingOverride : IEntityTypeConfiguration<Player>
{
    private const int MaxEmailLength = 320;

    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.Property<uint>("Version").IsRowVersion();
        
        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new PlayerId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username).IsRequired().HasMaxLength(MaxEmailLength);
        builder.Property(x => x.Nickname).IsRequired().HasMaxLength(MaxEmailLength)
            .HasConversion(to => to.Value, from => new Nickname(from));
    }
}

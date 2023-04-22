using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Persistence.Mappings;

internal sealed class ImageMappingOverride : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(x => x.Id).HasConversion(to => to.Value, from => new ImageId(from));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.GameRoomId).HasConversion(to => to.Value, from => new GameRoomId(from));

        builder.HasIndex(x => x.CreatedAt);

        builder.Property<uint>("Version").IsRowVersion();
    }
}

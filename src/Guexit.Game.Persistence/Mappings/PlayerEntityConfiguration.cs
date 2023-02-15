﻿using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TryGuessIt.Game.Persistence.Mappings;

internal sealed class PlayerEntityConfiguration : IEntityTypeConfiguration<Player>
{
    private const int MaxEmailLength = 320;

    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.Property(x => x.Id).HasConversion<PlayerIdValueConverter>();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username).IsRequired().HasMaxLength(MaxEmailLength);
        
        builder.Property<uint>("Version").IsRowVersion();
    }

    private sealed class PlayerIdValueConverter : ValueConverter<PlayerId, string>
    {
        public PlayerIdValueConverter()
            : base(
                to => to.Value,
                from => new PlayerId(from))
        {
        }
    }
}

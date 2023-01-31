using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TryGuessIt.Game.Persistence.Outbox;

namespace TryGuessIt.Game.Persistence.Mappings;

internal sealed class OutboxMessageEntityConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> modelBuilder)
    {
        modelBuilder.HasKey(x => x.Id);
        
        modelBuilder.Ignore(x => x.IsPublished);
        modelBuilder.HasIndex(x => x.PublishedAt)
            .IsUnique(false)
            .IsDescending(true);
        modelBuilder.HasIndex(x => x.CreatedAt)
            .IsUnique(false)
            .IsDescending(true);
    }
}


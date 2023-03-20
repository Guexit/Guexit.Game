using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentStateMap : SagaClassMap<DeckAssignmentState>
{
    protected override void Configure(EntityTypeBuilder<DeckAssignmentState> entity, ModelBuilder model)
    {
        entity.Property(x => x.LogicalShard);
        entity.Property(x => x.Version).IsRowVersion();
    }
}
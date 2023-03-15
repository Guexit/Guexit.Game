using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentSagaDbContext : SagaDbContext
{
    public DeckAssignmentSagaDbContext(DbContextOptions<DeckAssignmentSagaDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new DeckAssignmentStateMap();
        }
    }
}

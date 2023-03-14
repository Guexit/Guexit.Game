using Guexit.Game.Messages;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int LogicalShard { get; set; }
}

public sealed class DeckAssignmentSaga : MassTransitStateMachine<DeckAssignmentState>
{

    public DeckAssignmentSaga()
    {
        Event(() => AssignDeckCommand, @event => @event
            .CorrelateBy((instance, context) => instance.LogicalShard == context.Message.LogicalShard)
            .SelectId(x => NewId.NextGuid())
        );

        Initially(When(AssignDeckCommand)
            .Then(context => Console.WriteLine("Polla"))
            .TransitionTo(Completed));
    }

    public State Running { get; private set; } = default!;
    public State Completed { get; private set; } = default!;

    public Event<AssignDeckCommand> AssignDeckCommand { get; private set; } = default!;
}

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

public sealed class DeckAssignmentStateMap : SagaClassMap<DeckAssignmentState>
{
    protected override void Configure(EntityTypeBuilder<DeckAssignmentState> entity, ModelBuilder model)
    {
        entity.Property(x => x.LogicalShard);

        // If using Optimistic concurrency, otherwise remove this property
        //entity.Property(x => x.RowVersion).IsRowVersion();
    }
}
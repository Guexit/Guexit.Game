using MassTransit;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;

    public int LogicalShard { get; set; }
    public byte[] Version { get; set; } = default!;
}

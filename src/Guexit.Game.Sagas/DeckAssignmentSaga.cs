using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int LogicalShard { get; set; }
}

public sealed class DeckAssignmentSaga : MassTransitStateMachine<DeckAssignmentState>
{
    private readonly IDeckAssignmentService _deckAssignmentService;

    public DeckAssignmentSaga(IDeckAssignmentService deckAssignmentService)
    {
        _deckAssignmentService = deckAssignmentService;

        Event(() => AssignDeckCommand, @event => @event
            .CorrelateBy((instance, context) => instance.LogicalShard == context.Message.LogicalShard)
            .SelectId(x => NewId.NextGuid())
        );

        Initially(When(AssignDeckCommand)
            .ThenAsync(async (context) => await _deckAssignmentService.AssignDeck(context.Message.GameRoomId, context.CancellationToken))
            .TransitionTo(Completed));
    }

    public State Running { get; private set; } = default!;
    public State Completed { get; private set; } = default!;

    public Event<AssignDeckCommand> AssignDeckCommand { get; private set; } = default!;
}
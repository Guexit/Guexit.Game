using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Sagas;

public class AssignDeckCommandActivity : IStateMachineActivity<DeckAssignmentState, AssignDeckCommand>
{
    readonly IDeckAssignmentService _service;

    public AssignDeckCommandActivity(IDeckAssignmentService service)
    {
        _service = service;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("deck-assignment-command");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<DeckAssignmentState, AssignDeckCommand> context, IBehavior<DeckAssignmentState, AssignDeckCommand> next)
    {
        await _service.AssignDeck(context.Message.GameRoomId, context.CancellationToken);

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<DeckAssignmentState, AssignDeckCommand, TException> context,
        IBehavior<DeckAssignmentState, AssignDeckCommand> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}
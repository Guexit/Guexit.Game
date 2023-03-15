using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentSaga : MassTransitStateMachine<DeckAssignmentState>
{
    public DeckAssignmentSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => AssignDeckCommand, x =>
        {
            x.CorrelateBy<int>(state => state.LogicalShard, context => context.Message.LogicalShard);
            x.SelectId(context =>
            {
                //return new Guid(context.Message.LogicalShard, 0, 0, new byte[8]);
                return NewId.NextGuid();
            });
        });

        Initially(When(AssignDeckCommand)
            .TransitionTo(Running)
            .ThenAsync(async context =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                Console.WriteLine("Finishing saga...");
            })
            .Activity(x => x.OfType<AssignDeckCommandActivity>()));

        WhenEnter(Completed, (context) => context
            .ThenAsync(async (context) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(20));
            }));

        SetCompletedWhenFinalized();
    }

    public State Running { get; private set; } = default!;
    public State Completed { get; private set; } = default!;

    public Event<AssignDeckCommand> AssignDeckCommand { get; private set; } = default!;
}

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
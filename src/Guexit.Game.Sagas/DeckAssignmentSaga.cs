using Guexit.Game.Messages;
using MassTransit;

namespace Guexit.Game.Sagas;

public sealed class DeckAssignmentSaga : MassTransitStateMachine<DeckAssignmentState>
{
    private static readonly byte[] EmptyByteArray = new byte[8];

    public DeckAssignmentSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => AssignDeckCommand, x =>
        {
            x.CorrelateBy<int>(state => state.LogicalShard, context => context.Message.LogicalShard);
            x.SelectId(context => new Guid(context.Message.LogicalShard, 0, 0, EmptyByteArray));
        });

        Initially(When(AssignDeckCommand)
            .Then(x => x.Saga.LogicalShard = x.Message.LogicalShard)
            .TransitionTo(Running));

        During(Running, When(AssignDeckCommand));

        WhenEnter(Running, context => context.ThenAsync(async context =>
        {
            Console.WriteLine("Starting saga...");
            await Task.Delay(TimeSpan.FromSeconds(10));
            Console.WriteLine("Finishing saga...");
        }).Finalize());

        SetCompletedWhenFinalized();
    }

    public State Running { get; private set; } = default!;
    public State Completed { get; private set; } = default!;

    public Event<AssignDeckCommand> AssignDeckCommand { get; private set; } = default!;
}

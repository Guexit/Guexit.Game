using MassTransit;
using MassTransit.Testing;

namespace Guexit.Game.Component.IntegrationTests.Extensions;

public static class MasstransitExtensions
{
    public static async Task PublishAndWaitUntilConsumed<TMessage>(this ITestHarness harness, TMessage message) where TMessage : class
    {
        var id = Guid.NewGuid();
        
        await harness.Bus.Publish(message, new SetMessageIdPublishPipe<TMessage>(id));
        await WaitUntilConsumed<TMessage>(id, harness);
    }

    private static async Task WaitUntilConsumed<TMessage>(Guid messageId, ITestHarness harness) where TMessage : class
    {
        var timeout = TimeSpan.FromSeconds(5);
        var cts = new CancellationTokenSource(timeout);
        var cancellationToken = cts.Token;
        
        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));

        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            var hasBeenConsumed = await harness.Consumed.Any<TMessage>(x => x.Context.MessageId == messageId, cancellationToken);
            if (hasBeenConsumed)
                return;
        }
    }
    
    private sealed class SetMessageIdPublishPipe<TMessage>(Guid messageId) : IPipe<PublishContext<TMessage>> where TMessage : class
    {
        public Task Send(PublishContext<TMessage> context)
        {
            context.MessageId = messageId;
            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context) { }
    }
}
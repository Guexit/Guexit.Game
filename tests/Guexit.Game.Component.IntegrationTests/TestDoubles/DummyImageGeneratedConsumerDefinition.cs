using Guexit.Game.Consumers;
using MassTransit;

namespace Guexit.Game.Component.IntegrationTests.TestDoubles;

// TODO: remove testing against masstransit test harness
public sealed class DummyImageGeneratedConsumerDefinition : ImageGeneratedConsumerDefinition
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ImageGeneratedConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        // Left empty on purpose to set message to default masstransit config
    }
}
using MassTransit;
using TryGuessIt.IdentityProvider.WebApi.Events;

namespace TryGuessIt.Game.WebApi;

public class UserCreatedIntegrationEventConsumer : IConsumer<UserCreatedIntegrationEvent>
{
    private readonly ILogger<UserCreatedIntegrationEventConsumer> _logger;

    public UserCreatedIntegrationEventConsumer(ILogger<UserCreatedIntegrationEventConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
    {
        _logger.LogInformation("Handling UserCreatedIntegrationEvent. Id: '{userId}'", context.Message.Id);
        return Task.CompletedTask;
    }
}

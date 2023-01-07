using MassTransit;
using TryGuessIt.IdentityProvider.Messages;

namespace TryGuessIt.Game.WebApi;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreated> context)
    {
        _logger.LogInformation("Handling UserCreatedIntegrationEvent. Id: '{userId}'", context.Message.Id);
        return Task.CompletedTask;
    }
}

using MassTransit;
using Microsoft.Extensions.Logging;
using TryGuessIt.IdentityProvider.Messages;

namespace TryGuessIt.Game.ExternalMessageHandlers;

public sealed class UserCreatedHandler : IConsumer<UserCreated>
{
    private readonly ILogger<UserCreatedHandler> _logger;

    public UserCreatedHandler(ILogger<UserCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<UserCreated> context)
    {
        _logger.LogInformation("Handling UserCreatedIntegrationEvent. Id: '{userId}' Username: {username}", 
            context.Message.Id,
            context.Message.Username);
        return Task.CompletedTask;
    }
}

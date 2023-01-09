using MassTransit;
using Mediator;
using Microsoft.Extensions.Logging;
using TryGuessIt.Game.Application;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.IdentityProvider.Messages;

namespace TryGuessIt.Game.ExternalMessageHandlers;

public sealed class UserCreatedHandler : IConsumer<UserCreated>
{
    private readonly ILogger<UserCreatedHandler> _logger;
    private readonly ISender _sender;

    public UserCreatedHandler(ILogger<UserCreatedHandler> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        if (Random.Shared.Next(0, 4) != 3)
        {
            throw new Exception("Boom!");
        }

        _logger.LogInformation("Handling UserCreatedIntegrationEvent. Id: '{userId}' Username: {username}", 
            context.Message.Id,
            context.Message.Username);

        await _sender.Send(new CreatePlayerCommand(
            context.Message.Id, 
            context.Message.Username
        ), context.CancellationToken);
    }
}

using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.IdentityProvider.Messages;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ExternalMessageHandlers;

public sealed class UserCreatedHandler : ExternalMessageHandler<UserCreated>
{
    private readonly IPlayerManagementService _playerManagementService;

    public UserCreatedHandler(IPlayerManagementService playerManagementService, IUnitOfWork unitOfWork, ILogger<UserCreatedHandler> logger) 
        : base(unitOfWork, logger)
    {
        _playerManagementService = playerManagementService;
    }

    protected override async Task Process(UserCreated gameStarted, CancellationToken cancellationToken)
    {
        await _playerManagementService.CreatePlayer(new PlayerId(gameStarted.Id), gameStarted.Username, cancellationToken);
    }
}

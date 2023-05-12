using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.IdentityProvider.Messages;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.Consumers;

public sealed class UserCreatedConsumer : MessageConsumer<UserCreated>
{
    private readonly IPlayerManagementService _playerManagementService;

    public UserCreatedConsumer(IPlayerManagementService playerManagementService, IUnitOfWork unitOfWork, ILogger<UserCreatedConsumer> logger) 
        : base(unitOfWork, logger)
    {
        _playerManagementService = playerManagementService;
    }

    protected override async Task Process(UserCreated userCreated, CancellationToken cancellationToken)
    {
        await _playerManagementService.CreatePlayer(new PlayerId(userCreated.Id), userCreated.Username, cancellationToken);
    }
}

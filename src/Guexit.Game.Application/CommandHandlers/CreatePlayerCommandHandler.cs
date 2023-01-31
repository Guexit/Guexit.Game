using Guexit.Game.Application.Commands;
using Guexit.Game.Application.Services;
using Mediator;

namespace Guexit.Game.Application.CommandHandlers;
public sealed class CreatePlayerCommandHandler : CommandHandler<CreatePlayerCommand, Unit>
{
    private readonly IPlayerManagementService _playerManagementService;

    public CreatePlayerCommandHandler(IUnitOfWork unitOfWork, IPlayerManagementService playerManagementService) 
        : base(unitOfWork)
    {
        _playerManagementService = playerManagementService;
    }

    protected override async ValueTask<Unit> Process(CreatePlayerCommand command, CancellationToken cancellationToken)
    {
        await _playerManagementService.CreatePlayer(command.PlayerId, command.Username, cancellationToken);
        return Unit.Value;
    }
}

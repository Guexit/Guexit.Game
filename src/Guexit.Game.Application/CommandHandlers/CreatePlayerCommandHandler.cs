using Mediator;
using TryGuessIt.Game.Application.Commands;
using TryGuessIt.Game.Application.Services;

namespace TryGuessIt.Game.Application.CommandHandlers;
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

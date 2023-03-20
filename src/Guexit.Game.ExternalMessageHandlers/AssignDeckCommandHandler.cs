using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ExternalMessageHandlers;

public sealed class AssignDeckCommandHandler : ExternalMessageHandler<AssignDeckCommand>
{
    private readonly IDeckAssignmentService _deckAssignmentService;

    public AssignDeckCommandHandler(IDeckAssignmentService deckAssignmentService, IUnitOfWork unitOfWork, 
        ILogger<AssignDeckCommandHandler> logger) : base(unitOfWork, logger)
    {
        _deckAssignmentService = deckAssignmentService;
    }

    protected override async Task Process(AssignDeckCommand command, CancellationToken cancellationToken)
    {
        await _deckAssignmentService.AssignDeck(command.GameRoomId, cancellationToken);
    }
}
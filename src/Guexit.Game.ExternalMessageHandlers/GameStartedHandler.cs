using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ExternalMessageHandlers;

public sealed class GameStartedHandler : ExternalMessageHandler<GameStartedIntegrationEvent>
{
    private readonly IDeckAssignmentService _deckAssignmentService;

    public GameStartedHandler(IDeckAssignmentService deckAssignmentService, IUnitOfWork unitOfWork, 
        ILogger<GameStartedHandler> logger) : base(unitOfWork, logger)
    {
        _deckAssignmentService = deckAssignmentService;
    }

    protected override async Task Process(GameStartedIntegrationEvent gameStarted, CancellationToken cancellationToken)
    {
        await _deckAssignmentService.AssignDeck(gameStarted.GameRoomId, cancellationToken);
    }
}
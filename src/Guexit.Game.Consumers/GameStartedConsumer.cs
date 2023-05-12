using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Messages;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.Consumers;

public sealed class GameStartedConsumer : MessageConsumer<GameStartedIntegrationEvent>
{
    private readonly IDeckAssignmentService _deckAssignmentService;

    public GameStartedConsumer(IDeckAssignmentService deckAssignmentService, IUnitOfWork unitOfWork, 
        ILogger<GameStartedConsumer> logger) : base(unitOfWork, logger)
    {
        _deckAssignmentService = deckAssignmentService;
    }

    protected override async Task Process(GameStartedIntegrationEvent gameStarted, CancellationToken cancellationToken)
    {
        await _deckAssignmentService.AssignDeck(gameStarted.GameRoomId, cancellationToken);
    }
}
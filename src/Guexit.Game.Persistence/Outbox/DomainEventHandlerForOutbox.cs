using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Messages;

namespace TryGuessIt.Game.Persistence.Outbox;

public sealed class DomainEventHandlerForOutbox : IDomainEventHandler<PlayerJoinedGameRoom>
{
    private readonly GameDbContext _dbContext;
    private readonly OutboxMessageFactory _outboxMessageFactory;

    public DomainEventHandlerForOutbox(GameDbContext dbContext, OutboxMessageFactory messageFactory)
    {
        _dbContext = dbContext;
        _outboxMessageFactory = messageFactory;
    }

    public async ValueTask Handle(PlayerJoinedGameRoom @event, CancellationToken ct)
    {
        await AddToOutbox(new PlayerJoinedGameRoomIntegrationEvent
        {
            PlayerId = @event.PlayerId,
            GameRoomId = @event.GameRoomId
        }, ct);
    }

    private async ValueTask AddToOutbox<TMessage>(TMessage message, CancellationToken ct)
    {
        var outboxMessage = _outboxMessageFactory.CreateFrom(message);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, ct);
    }
}
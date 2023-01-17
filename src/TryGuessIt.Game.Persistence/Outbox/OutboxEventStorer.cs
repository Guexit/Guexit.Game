using TryGuessIt.Game.Domain;
using TryGuessIt.Game.Domain.Model.GameRoomAggregate.Events;

namespace TryGuessIt.Game.Persistence.Outbox;

public sealed class OutboxEventStorer : IDomainEventHandler<PlayerJoinedGameRoom>
{
    private readonly GameDbContext _dbContext;
    private readonly OutboxMessageFactory _outboxMessageFactory;

    public OutboxEventStorer(GameDbContext dbContext, OutboxMessageFactory messageFactory)
    {
        _dbContext = dbContext;
        _outboxMessageFactory = messageFactory;
    }

    public async ValueTask Handle(PlayerJoinedGameRoom @event, CancellationToken ct) => await AddToOutbox(@event, ct);

    private async ValueTask AddToOutbox<TMessage>(TMessage message, CancellationToken cancellationToken)
        where TMessage : IDomainEvent
    {
        var outboxMessage = _outboxMessageFactory.CreateFrom(message);
        await _dbContext.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
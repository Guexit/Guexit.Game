using System.Text.Json;
using TryGuessIt.Game.Domain;
using ISystemClock = TryGuessIt.Game.Domain.ISystemClock;

namespace TryGuessIt.Game.Persistence.Outbox;

public sealed class OutboxMessageFactory
{
    private readonly ISystemClock _clock;
    private readonly IGuidProvider _guidProvider;

    public OutboxMessageFactory(ISystemClock clock, IGuidProvider guidProvider)
    {
        _clock = clock;
        _guidProvider = guidProvider;
    }

    public OutboxMessage CreateFrom<TMessage>(TMessage message)
    {
        var type = typeof(TMessage).FullName;
        var serializedMessage = JsonSerializer.Serialize(message);

        return new OutboxMessage(_guidProvider.NewGuid(), type, serializedMessage, _clock.UtcNow);
    }
}

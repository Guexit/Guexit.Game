using Guexit.Game.Application.CardAssigment;
using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application.Services;

public interface IDeckAssignmentService
{
    Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default);
}

public class DeckAssignmentService : IDeckAssignmentService
{
    private readonly ILogicalShardDistributedLock _logicalShardDistributedLock;
    private readonly ILogicalShardProvider _logicalShardProvider;

    public DeckAssignmentService(ILogicalShardDistributedLock logicalShardDistributedLock, ILogicalShardProvider logicalShardProvider)
    {
        _logicalShardDistributedLock = logicalShardDistributedLock;
        _logicalShardProvider = logicalShardProvider;
        
    }

    public async Task AssignDeck(GameRoomId gameRoomId, CancellationToken cancellationToken = default)
    {
        var logicalShard = _logicalShardProvider.GetLogicalShard();
        
        try
        {
            await _logicalShardDistributedLock.Acquire(logicalShard, cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
        finally
        {
            await _logicalShardDistributedLock.Release(logicalShard, cancellationToken);
        }
    }
}

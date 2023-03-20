namespace Guexit.Game.Application.CardAssigment;

public interface ILogicalShardDistributedLock : IAsyncDisposable
{
    Task Acquire(int logicalShard, CancellationToken ct = default);
    Task Release(int logicalShard, CancellationToken ct = default);
}

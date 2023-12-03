using System.Data.Common;

namespace Guexit.Game.Application;

public interface IUnitOfWork
{
    Task BeginTransaction(CancellationToken cancellationToken = default);
    Task Commit(CancellationToken cancellationToken = default);
    Task Rollback(CancellationToken cancellationToken = default);
}

using System.Data.Common;

namespace Guexit.Game.Application;

public interface IUnitOfWork
{
    Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken = default);
    Task SaveChanges(CancellationToken cancellationToken = default);
}

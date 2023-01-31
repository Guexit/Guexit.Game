namespace Guexit.Game.Application;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken = default);
}

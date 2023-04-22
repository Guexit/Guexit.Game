using System.Data;
using Guexit.Game.Application;
using Guexit.Game.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace Guexit.Game.Persistence;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly GameDbContext _dbContext;
    private readonly IDomainEventPublisher _domainEventPublisher;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(GameDbContext dbContext, IDomainEventPublisher domainEventPublisher)
    {
        _dbContext = dbContext;
        _domainEventPublisher = domainEventPublisher;
    }

    public async Task BeginTransaction(CancellationToken cancellationToken)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction already began, multiple transactions is not supported");

        _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Cannot rollback commit before begining a transaction");

        await PublishDomainEvents(cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task Rollback(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Cannot rollback unit of work before begining a transaction");

        await _transaction.RollbackAsync(cancellationToken);
    }

    private async ValueTask PublishDomainEvents(CancellationToken cancellationToken)
    {
        var aggregateRoots = _dbContext.ChangeTracker.Entries<IAggregateRoot>().Select(x => x.Entity).ToArray();
        var domainEvents = aggregateRoots.SelectMany(x => x.DomainEvents).ToArray();

        while (domainEvents.Length > 0)
        {
            foreach (var aggregateRoot in aggregateRoots) 
                aggregateRoot.ClearDomainEvents();

            await _domainEventPublisher.Publish(domainEvents, cancellationToken);
            
            aggregateRoots = _dbContext.ChangeTracker.Entries<IAggregateRoot>().Select(x => x.Entity).ToArray();
            domainEvents = aggregateRoots.SelectMany(x => x.DomainEvents).ToArray();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null)
            await _transaction.DisposeAsync();
    }
}

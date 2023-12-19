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

    public async Task BeginTransaction(CancellationToken ct = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("Another transaction already started. Multiple transactions are not supported.");

        _transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task Commit(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Cannot commit because no transaction was opened. A transaction must be open before committing");

        await PublishDomainEvents(ct);
        await _dbContext.SaveChangesAsync(ct);
        await _transaction.CommitAsync(ct);
    }

    public async Task Rollback(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Cannot rollback because no transaction was opened. A transaction must be open before rollback");

        await _transaction.RollbackAsync(ct);
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
        if (_transaction is null)
            return;

        await _transaction.DisposeAsync();
    }
}
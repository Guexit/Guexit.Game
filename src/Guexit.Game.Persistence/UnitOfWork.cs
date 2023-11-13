using System.Data;
using System.Data.Common;
using Guexit.Game.Application;
using Guexit.Game.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace Guexit.Game.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _dbContext;
    private readonly IDomainEventPublisher _domainEventPublisher;

    public UnitOfWork(GameDbContext dbContext, IDomainEventPublisher domainEventPublisher)
    {
        _dbContext = dbContext;
        _domainEventPublisher = domainEventPublisher;
    }

    public async Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return dbContextTransaction.GetDbTransaction();
    }

    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await PublishDomainEvents(cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
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
}

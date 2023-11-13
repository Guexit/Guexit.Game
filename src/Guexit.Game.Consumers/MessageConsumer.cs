using Guexit.Game.Application;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.Consumers;

public abstract class MessageConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IConsumer<TMessage>> _logger;

    protected MessageConsumer(IUnitOfWork unitOfWork, ILogger<IConsumer<TMessage>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<TMessage> consumeContext)
    {
        await using var transaction = await _unitOfWork.BeginTransaction(consumeContext.CancellationToken);
        
        try
        {
            _logger.LogInformation("Handling external message of type {MessageTypeName}", typeof(TMessage).Name);
            
            await Process(consumeContext.Message, consumeContext.CancellationToken);
            await _unitOfWork.SaveChanges(consumeContext.CancellationToken);
            await transaction.CommitAsync(consumeContext.CancellationToken);
            
            _logger.LogInformation("{MessageTypeName} processed successfully", typeof(TMessage).Name);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error handling external message {MessageTypeName}", typeof(TMessage).Name);
            await transaction.RollbackAsync(consumeContext.CancellationToken);
            throw;
        }
    }

    protected abstract Task Process(TMessage message, CancellationToken cancellationToken);
}
using Guexit.Game.Application;
using Mediator;

namespace Guexit.Game.Persistence.Interceptors;

public sealed class UnitOfWorkCommandPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : Application.ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkCommandPipelineBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async ValueTask<TResponse> Handle(TCommand command, CancellationToken ct, MessageHandlerDelegate<TCommand, TResponse> next)
    {
        await _unitOfWork.BeginTransaction(ct);

        try
        {
            var response = await next(command, ct);
            
            await _unitOfWork.Commit(ct);
            
            return response;
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback(ct);
            throw;
        }
    }
}
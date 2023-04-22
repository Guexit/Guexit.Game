using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand, Unit>
    where TCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;

    protected CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            await Process(command, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback(cancellationToken);
            throw;
        }
       
        return Unit.Value;
    }

    protected abstract ValueTask Process(TCommand command, CancellationToken cancellationToken);
}

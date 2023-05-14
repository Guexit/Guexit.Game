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

    public async ValueTask<Unit> Handle(TCommand command, CancellationToken ct = default)
    {
        await _unitOfWork.BeginTransaction(ct);

        try
        {
            await Process(command, ct);
            await _unitOfWork.Commit(ct);
        }
        catch (Exception)
        {
            await _unitOfWork.Rollback(ct);
            throw;
        }
       
        return Unit.Value;
    }

    protected abstract ValueTask Process(TCommand command, CancellationToken ct);
}

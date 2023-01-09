using Mediator;

namespace TryGuessIt.Game.Application;


public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;

    public CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            await Process(command, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return Unit.Value;
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected abstract ValueTask Process(TCommand command, CancellationToken cancellationToken);
}

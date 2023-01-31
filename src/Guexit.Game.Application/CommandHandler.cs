using Mediator;

namespace TryGuessIt.Game.Application;

public abstract class CommandHandler<TCommand, TCommandCompletion> : IRequestHandler<TCommand, TCommandCompletion>
    where TCommand : ICommand<TCommandCompletion>
    where TCommandCompletion : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    public CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<TCommandCompletion> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Process(command, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected abstract ValueTask<TCommandCompletion> Process(TCommand command, CancellationToken cancellationToken);
}

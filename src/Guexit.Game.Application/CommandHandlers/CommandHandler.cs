using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public abstract class CommandHandler<TCommand, TCommandCompletion> : IRequestHandler<TCommand, TCommandCompletion>
    where TCommand : ICommand<TCommandCompletion>
    where TCommandCompletion : notnull
{
    private readonly IUnitOfWork _unitOfWork;

    protected CommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<TCommandCompletion> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        var result = await Process(command, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return result;
    }

    protected abstract ValueTask<TCommandCompletion> Process(TCommand command, CancellationToken cancellationToken);
}

using Mediator;

namespace TryGuessIt.Game.Application;

public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
    public ValueTask<Unit> Handle(TCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using Mediator;

namespace Guexit.Game.Application;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out TCommandCompletion> : IRequest<TCommandCompletion>
    where TCommandCompletion : notnull
{
}
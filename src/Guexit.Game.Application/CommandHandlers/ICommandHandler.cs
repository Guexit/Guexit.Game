using Mediator;

namespace Guexit.Game.Application.CommandHandlers;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit> where TCommand : ICommand
{
}

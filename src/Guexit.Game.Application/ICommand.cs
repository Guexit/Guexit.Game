using Mediator;

namespace Guexit.Game.Application;

internal interface ICommand : ICommand<Unit>;

public interface ICommand<out TResponse> : IRequest<TResponse>;

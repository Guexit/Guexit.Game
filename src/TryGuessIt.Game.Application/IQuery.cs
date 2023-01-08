using Mediator;

namespace TryGuessIt.Game.Application;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

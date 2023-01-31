using Mediator;

namespace TryGuessIt.Game.ReadModels.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

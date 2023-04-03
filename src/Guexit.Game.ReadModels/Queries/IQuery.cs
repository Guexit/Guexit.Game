using Mediator;

namespace Guexit.Game.ReadModels.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}

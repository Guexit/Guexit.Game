using Mediator;

namespace Guexit.Game.ReadModels;

public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull;

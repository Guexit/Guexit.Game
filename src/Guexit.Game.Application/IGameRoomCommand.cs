using Guexit.Game.Domain.Model.GameRoomAggregate;
using Mediator;

namespace Guexit.Game.Application;

internal interface IGameRoomCommand : IGameRoomCommand<Unit>;

public interface IGameRoomCommand<out TResponse> : ICommand<TResponse>
{
    public GameRoomId GameRoomId { get; }
}
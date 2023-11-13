using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Application;

/// <summary>
/// Represents a command that operates within a game room context. 
/// It includes an optimistic concurrency check feature to ensure concurrency token is checked
/// when multiple players execute commands simultaneously in the same game room.
/// </summary>
public interface IGameRoomCommand : ICommand
{
    public GameRoomId GameRoomId { get; }
}
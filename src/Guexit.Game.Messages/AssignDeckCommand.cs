namespace Guexit.Game.Messages;

public sealed class AssignDeckCommand
{
    public Guid GameRoomId { get; init; }
    public int LogicalShard { get; init; } = 1;
}

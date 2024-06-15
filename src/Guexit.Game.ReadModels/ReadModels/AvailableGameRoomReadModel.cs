namespace Guexit.Game.ReadModels.ReadModels;

public sealed class AvailableGameRoomReadModel
{
    public required Guid GameRoomId { get; init; }
    public required int RequiredMinPlayers { get; init; }
    public required int CurrentPlayerCount { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

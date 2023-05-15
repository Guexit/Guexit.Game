namespace Guexit.Game.ReadModels.ReadModels;

public sealed class LobbyReadModel
{
    public required Guid GameRoomId { get; init; }
    public required int RequiredMinPlayers { get; init; }
    public required LobbyPlayerDto[] Players { get; init; }
    public required bool CanStartGame { get; init; }
    public required string GameStatus { get; init; }
}

public sealed class LobbyPlayerDto
{
    public required string Username { get; init; }
}

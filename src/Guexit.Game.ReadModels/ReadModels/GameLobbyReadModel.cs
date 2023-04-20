namespace Guexit.Game.ReadModels.ReadModels;

public sealed class GameLobbyReadModel
{
    public required Guid GameRoomId { get; init; }
    public required int RequiredMinPlayers { get; init; }
    public required GameLobbyPlayerDto[] Players { get; init; }
    public required bool CanStartGame { get; init; }
    public required string GameStatus { get; init; }
}

public sealed class GameLobbyPlayerDto
{
    public required string Username { get; init; }
}
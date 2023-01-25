namespace TryGuessIt.Game.ReadModels.ReadModels;

public sealed class GameLobbyReadModel
{
    public Guid GameRoomId { get; }
    public int RequiredMinPlayers { get; }
    public GameLobbyPlayerDto[] Players { get; }

    public GameLobbyReadModel(Guid gameRoomId, int requiredMinPlayers, GameLobbyPlayerDto[] players)
    {
        GameRoomId = gameRoomId;
        RequiredMinPlayers = requiredMinPlayers;
        Players = players;
    }
}

public sealed class GameLobbyPlayerDto
{
    public string Username { get; }

    public GameLobbyPlayerDto(string username)
    {
        Username = username;
    }
}
namespace TryGuessIt.Game.ReadModels.ReadModels;

public sealed class GameLobbyReadModel
{
    public int RequiredMinPlayers { get; }
    public GameLobbyPlayerDto[] Players { get; }

    public GameLobbyReadModel(int requiredMinPlayers, GameLobbyPlayerDto[] players)
    {
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
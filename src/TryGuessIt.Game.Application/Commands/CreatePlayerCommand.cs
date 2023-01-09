namespace TryGuessIt.Game.Application.Commands;

public sealed class CreatePlayerCommand : ICommand
{
    public string PlayerId { get; }
    public string Username { get; }

    public CreatePlayerCommand(string playerId, string username)
    {
        PlayerId = playerId;
        Username = username;
    }
}

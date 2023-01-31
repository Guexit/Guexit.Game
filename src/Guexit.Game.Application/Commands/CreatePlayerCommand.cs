using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class CreatePlayerCommand : ICommand
{
    public PlayerId PlayerId { get; }
    public string Username { get; }

    public CreatePlayerCommand(string playerId, string username)
    {
        PlayerId = new PlayerId(playerId);
        Username = username;
    }
}

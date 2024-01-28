using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Application.Commands;

public sealed class ChangePlayerNicknameCommand : ICommand
{
    public PlayerId PlayerId { get; }
    public Nickname Nickname { get; }

    public ChangePlayerNicknameCommand(string userId, string nickname)
    {
        PlayerId = new PlayerId(userId);
        Nickname = new Nickname(nickname);
    }
}
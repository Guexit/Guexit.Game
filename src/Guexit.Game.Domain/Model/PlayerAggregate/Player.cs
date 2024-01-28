using Guexit.Game.Domain.Exceptions;

namespace Guexit.Game.Domain.Model.PlayerAggregate;

public sealed class Player : AggregateRoot<PlayerId>
{
    public Nickname Nickname { get; private set; } = default!;
    public string Username { get; private init; } = default!;

    private Player()
    {
        // Entity Framework required parameterless ctor
    }

    public Player(PlayerId id, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new EmptyUsernameException();
        
        Id = id;
        Username = username;
        Nickname = Nickname.From(Username);
    }

    public void ChangeNickname(Nickname nickname)
    {
        Nickname = nickname;
    }
}
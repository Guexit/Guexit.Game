using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Tests.Common.Builders;

public sealed class PlayerBuilder
{
    private PlayerId _id = new(Guid.NewGuid().ToString());
    private string _username = Guid.NewGuid().ToString();

    public Player Build()
    {
        return new Player(_id, _username);
    }

    public PlayerBuilder WithId(PlayerId id)
    {
        _id = id;
        return this;
    }

    public PlayerBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }
}

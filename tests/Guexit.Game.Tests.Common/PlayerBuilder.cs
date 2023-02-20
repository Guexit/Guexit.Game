﻿using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Tests.Common;

public sealed class PlayerBuilder
{
    private PlayerId _id = new(Guid.Empty.ToString());
    private string _username = string.Empty;

    public Player Build()
    {
        return new Player(_id, _username);
    }
    
    public PlayerBuilder WithId(string id)
    {
        _id = new PlayerId(id);
        return this;
    }

    public PlayerBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }
}

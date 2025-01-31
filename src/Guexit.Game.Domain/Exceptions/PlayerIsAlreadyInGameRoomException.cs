﻿using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class PlayerIsAlreadyInGameRoomException : DomainException
{
    public override string Title => "Player is already in game room";

    public PlayerIsAlreadyInGameRoomException(PlayerId playerId)
        : base($"Player with id {playerId.Value} is already in game room.")
    {

    }
}

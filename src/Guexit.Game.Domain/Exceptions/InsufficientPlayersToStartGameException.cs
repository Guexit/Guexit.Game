﻿using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class InsufficientPlayersToStartGameException : DomainException
{
    public override string Title => "Insufficient number players to start game";


    public InsufficientPlayersToStartGameException(GameRoomId gameRoomId, int playersCount, RequiredMinPlayers requiredMinPlayers) 
        : base($"Game room {gameRoomId.Value} requires a minimum of {requiredMinPlayers.Count} players to start, but only {playersCount} players are present.")
    {
    }
}

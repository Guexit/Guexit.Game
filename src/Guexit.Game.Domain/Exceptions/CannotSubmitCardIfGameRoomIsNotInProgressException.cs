﻿using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotSubmitCardIfGameRoomIsNotInProgressException : DomainException
{
    public override string Title { get; } = "Cannot submit card to a not started game";

    public CannotSubmitCardIfGameRoomIsNotInProgressException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Cannot submit card to game with id {gameRoomId.Value} because is not in progress";
}

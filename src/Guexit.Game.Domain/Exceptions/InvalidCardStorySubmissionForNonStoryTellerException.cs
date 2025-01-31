﻿using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class InvalidCardStorySubmissionForNonStoryTellerException : DomainException
{
    public override string Title => "Cannot submit card story because player is not the current story teller";

    public InvalidCardStorySubmissionForNonStoryTellerException(GameRoomId gameRoomId, PlayerId playerId)
        : base(BuildExceptionMessage(gameRoomId, playerId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId playerId) 
        => $"Cannot submit card story to game room with id {gameRoomId.Value} because player with id {playerId.Value} is not the current story teller";
}

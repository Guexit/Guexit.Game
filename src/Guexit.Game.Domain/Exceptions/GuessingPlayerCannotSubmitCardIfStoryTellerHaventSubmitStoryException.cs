﻿using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class GuessingPlayerCannotSubmitCardIfStoryTellerHaventSubmitStoryException : DomainException
{
    public override string Title { get; } = "Guessing player cannot submit card if story teller hasn't submitted story";

    public GuessingPlayerCannotSubmitCardIfStoryTellerHaventSubmitStoryException(GameRoomId gameRoomId, PlayerId playerId)
        : base(BuildExceptionMessage(gameRoomId, playerId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId playerId)
        => $"Guessing player with id {playerId.Value} cannot submit card in game room with id {gameRoomId.Value} because story teller hasn't submitted story";
}
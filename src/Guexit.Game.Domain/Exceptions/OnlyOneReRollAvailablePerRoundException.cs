using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class OnlyOneReRollAvailablePerRoundException : DomainException
{
    public override string Title => "Player already re-rolled a card this round";

    public OnlyOneReRollAvailablePerRoundException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} already re-rolled one card this round in game room with id {gameRoomId}. Only 1 re-roll is allowed per round.")
    {
    }
}
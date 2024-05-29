using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.ReadModels.Exceptions;

public sealed class CardsForReRollUnavailableException : QueryException
{
    public override string Title => "No cards for re-roll currently available";

    private CardsForReRollUnavailableException(string message) : base(message) { }

    public static CardsForReRollUnavailableException AlreadyCompleted(GameRoomId gameRoomId, PlayerId playerId)
        => new($"Player with id {playerId.Value} already completed a card re-roll for this round in game room {gameRoomId.Value}.");

    public static CardsForReRollUnavailableException NotReserved(GameRoomId gameRoomId, PlayerId playerId)
        => new($"Player with id {playerId.Value} has not reserved a card re-roll for this round in game room {gameRoomId.Value}.");
}
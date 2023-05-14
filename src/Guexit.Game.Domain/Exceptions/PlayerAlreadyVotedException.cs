using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class PlayerAlreadyVotedException : DomainException
{
    public override string Title => "Player already voted a card";

    public PlayerAlreadyVotedException(GameRoomId gameRoomId, PlayerId playerId)
        : base($"Player with id {playerId.Value} cannot vote another card because already voted in current round in game room with id {gameRoomId.Value}")
    {

    }
}
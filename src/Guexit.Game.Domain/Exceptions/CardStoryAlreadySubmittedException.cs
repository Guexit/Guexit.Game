using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CardStoryAlreadySubmittedException : DomainException
{
    public override string Title => "Storyteller already submitted card and story";

    public CardStoryAlreadySubmittedException(GameRoomId gameRoomId, PlayerId playerId)
        : base(BuildExceptionMessage(gameRoomId, playerId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId, PlayerId playerId) 
        => $"Storyteller with id {playerId.Value} already submitted card story in game room with id {gameRoomId.Value}";
}

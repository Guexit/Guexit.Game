using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class CannotLinkToNextGameRoomIfItsNotFinishedException : DomainException
{
    public override string Title => "Cannot link to next game room cause current one is not finished";

    public CannotLinkToNextGameRoomIfItsNotFinishedException(GameRoomId gameRoomId)
        : base($"Cannot link to the next game for game room with id {gameRoomId.Value} because the current game's status is not finished.")
    {
    }
}
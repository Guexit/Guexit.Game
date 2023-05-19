using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public class StartAlreadyStartedGameException : DomainException
{
    public override string Title => "Cannot start already started game exception";

    public StartAlreadyStartedGameException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Game with id {gameRoomId.Value} cannot be started because it's already started";
}

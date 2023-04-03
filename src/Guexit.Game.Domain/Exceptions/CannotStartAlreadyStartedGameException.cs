using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Exceptions;

public class CannotStartAlreadyStartedGameException : DomainException
{
    public override string Title { get; } = "Cannot start already started game exception";

    public CannotStartAlreadyStartedGameException(GameRoomId gameRoomId)
        : base(BuildExceptionMessage(gameRoomId))
    {
    }

    private static string BuildExceptionMessage(GameRoomId gameRoomId) 
        => $"Game with id {gameRoomId.Value} cannot be started because it's already started";
}

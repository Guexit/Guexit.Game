using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.ReadModels.Exceptions;

public sealed class CannotReadBoardIfGameIsNotInProgressException : QueryException
{
    public override string Title { get; } = "Cannot read board if game is not in progress";

    public CannotReadBoardIfGameIsNotInProgressException(GameRoomId gameRoomId, GameStatus status)
        : base($"Cannot read board for game room with id {gameRoomId.Value} because game is not in progress. Actual status: {status.Value}")
    {
    }
}
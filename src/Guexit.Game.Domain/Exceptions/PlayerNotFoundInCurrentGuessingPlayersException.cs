using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class PlayerNotFoundInCurrentGuessingPlayersException : DomainException
{
    public override string Title => "Player not found in the current guessing players";

    public PlayerNotFoundInCurrentGuessingPlayersException(PlayerId playerId)
        : base(BuildExceptionMessage(playerId))
    {
    }

    private static string BuildExceptionMessage(PlayerId playerId)
        => $"Cannot submit card for player with id {playerId.Value} because they are not in the current guessing players";
}

using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.Domain.Model.PlayerAggregate;

namespace TryGuessIt.Game.Application.Commands;

public sealed record class CreateGameRoomCommandCompletion(GameRoomId GameRoomId);

public sealed class CreateGameRoomCommand : ICommand<CreateGameRoomCommandCompletion>
{
    public PlayerId PlayerId { get; }

	public CreateGameRoomCommand(string playerId)
    {
        PlayerId = new PlayerId(playerId);
    }
}

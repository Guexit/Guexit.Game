using TryGuessIt.Game.Application.Commands;

namespace TryGuessIt.Game.WebApi.Dtos.Request;

public class CreateGameRoomCompletion
{
	public Guid GameRoomId { get; }

	private CreateGameRoomCompletion(Guid gameRoomId)
	{
        GameRoomId = gameRoomId;
    }

	public static CreateGameRoomCompletion From(CreateGameRoomCommandCompletion completion)
	{
		return new(completion.GameRoomId.Value);
	}
}

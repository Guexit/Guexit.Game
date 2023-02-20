using Guexit.Game.Application.Commands;

namespace Guexit.Game.WebApi.Dtos.Request;

public class CreateGameRoomResponseDto
{
	public Guid GameRoomId { get; }

	private CreateGameRoomResponseDto(Guid gameRoomId)
	{
        GameRoomId = gameRoomId;
    }

	public static CreateGameRoomResponseDto From(CreateGameRoomCommandCompletion completion) => new(completion.GameRoomId.Value);
}

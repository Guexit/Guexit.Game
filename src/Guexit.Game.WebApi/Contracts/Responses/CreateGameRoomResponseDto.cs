using Guexit.Game.Application.Commands;

namespace Guexit.Game.WebApi.Contracts.Responses;

// TODO: send id of game room id from the UI and remove this response
public class CreateGameRoomResponseDto
{
    public Guid GameRoomId { get; }

    private CreateGameRoomResponseDto(Guid gameRoomId)
    {
        GameRoomId = gameRoomId;
    }

    public static CreateGameRoomResponseDto From(CreateGameRoomCommandCompletion completion) => new(completion.GameRoomId);
}

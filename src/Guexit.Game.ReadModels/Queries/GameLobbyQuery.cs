using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.ReadModels.ReadModels;

namespace Guexit.Game.ReadModels.Queries;

public sealed class GameLobbyQuery : IQuery<GameLobbyReadModel>
{
    public GameRoomId GameRoomId { get; }

    public GameLobbyQuery(Guid gameRoomId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
    }
}

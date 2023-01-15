using TryGuessIt.Game.Domain.Model.GameRoomAggregate;
using TryGuessIt.Game.ReadModels.ReadModels;

namespace TryGuessIt.Game.ReadModels.Queries;

public sealed class GameLobbyQuery : IQuery<GameLobbyReadModel>
{
    public GameRoomId GameRoomId { get; }

    public GameLobbyQuery(GameRoomId gameRoomId)
    {
        GameRoomId = gameRoomId;
    }
}

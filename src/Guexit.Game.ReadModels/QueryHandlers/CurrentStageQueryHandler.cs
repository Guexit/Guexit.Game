using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.Exceptions;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class CurrentStageQuery : IQuery<GameStageReadModel> 
{ 
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public CurrentStageQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class CurrentStageQueryHandler : IQueryHandler<CurrentStageQuery, GameStageReadModel>
{
    private static readonly IGameStageSpecification[] _allGameStageSpecifications =
    [
        new BoardSpecification(),
        new VotingSpecification(),
        new LobbySpecification(),
        new EndSpecification()
    ];

    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;

    public CurrentStageQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<GameStageReadModel> Handle(CurrentStageQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var matchingStageSpecification = _allGameStageSpecifications.FirstOrDefault(x => x.IsSatisfiedBy(gameRoom));
        if (matchingStageSpecification is null)
            throw new CouldNotMatchCurrentStageException(query.GameRoomId, query.PlayerId);

        return new GameStageReadModel
        {
            GameRoomId = query.GameRoomId,
            CurrentStage = matchingStageSpecification.Stage.Value
        };
    }
}

public interface IGameStageSpecification
{
    GameStage Stage { get; }
    bool IsSatisfiedBy(GameRoom gameRoom);
}

public sealed class LobbySpecification : IGameStageSpecification
{
    public GameStage Stage => GameStage.Lobby;

    public bool IsSatisfiedBy(GameRoom gameRoom) => gameRoom.Status == GameStatus.NotStarted;
}

public sealed class BoardSpecification : IGameStageSpecification
{
    public GameStage Stage => GameStage.Board;

    public bool IsSatisfiedBy(GameRoom gameRoom)
    {
        return gameRoom.Status == GameStatus.InProgress && gameRoom.SubmittedCards.Count != gameRoom.GetPlayersCount();
    }
}   

public sealed class VotingSpecification : IGameStageSpecification
{
    public GameStage Stage => GameStage.Voting;

    public bool IsSatisfiedBy(GameRoom gameRoom)
    {
        return gameRoom.Status == GameStatus.InProgress && gameRoom.SubmittedCards.Count == gameRoom.GetPlayersCount();
    }
}  

public sealed class EndSpecification : IGameStageSpecification
{
    public GameStage Stage => GameStage.End;

    public bool IsSatisfiedBy(GameRoom gameRoom) => gameRoom.Status == GameStatus.Finished;
}
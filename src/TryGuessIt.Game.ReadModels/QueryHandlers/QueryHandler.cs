using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TryGuessIt.Game.Persistence;
using TryGuessIt.Game.ReadModels.Queries;
using TryGuessIt.Game.ReadModels.ReadModels;

namespace TryGuessIt.Game.ReadModels.QueryHandlers;

public sealed class GameLobbyQueryHandler : QueryHandler<GameLobbyQuery, GameLobbyReadModel>
{
    public GameLobbyQueryHandler(GameDbContext dbContext, ILogger<QueryHandler<GameLobbyQuery, GameLobbyReadModel>> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<GameLobbyReadModel> Process(GameLobbyQuery query, CancellationToken cancellationToken)
    {
        var gameRoom = await DbContext.GameRooms.FirstAsync(x => x.Id == query.GameRoomId, cancellationToken: cancellationToken);
        var playersInGame = await DbContext.Players.Where(x => gameRoom.PlayerIds.Contains(x.Id)).ToArrayAsync(cancellationToken);

        var gameLobbyReadModel = new GameLobbyReadModel(
            gameRoom.RequiredMinPlayers.Count, 
            playersInGame.Select(x => new GameLobbyPlayerDto(x.Username)).ToArray()
        );
        return gameLobbyReadModel;
    }
}

public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : Queries.IQuery<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<QueryHandler<TQuery, TResponse>> _logger;

    protected GameDbContext DbContext { get; }

    public QueryHandler(GameDbContext dbContext, ILogger<QueryHandler<TQuery, TResponse>> logger)
    {
        DbContext = dbContext;
        _logger = logger;

        DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public async ValueTask<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Handling {queryType}", typeof(TQuery).Name);
            return await Process(query, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while handling {queryType}", typeof(TQuery).Name);
            throw;
        }
    }

    protected abstract Task<TResponse> Process(TQuery query, CancellationToken cancellationToken);
}

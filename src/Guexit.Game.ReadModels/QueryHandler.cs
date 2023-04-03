using Guexit.Game.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ReadModels;

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

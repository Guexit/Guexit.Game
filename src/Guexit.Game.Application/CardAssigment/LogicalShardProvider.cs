using Microsoft.Extensions.Options;

namespace Guexit.Game.Application.CardAssigment;

public sealed class LogicalShardProvider : ILogicalShardProvider
{
    private readonly IOptions<CardAssignmentOptions> _options;

    public LogicalShardProvider(IOptions<CardAssignmentOptions> options)
    {
        _options = options;
    }

    public int GetLogicalShard()
    {
        var logicalShard = Random.Shared.Next(1, _options.Value.LogicalShardsCount);
        return logicalShard;
    }
}

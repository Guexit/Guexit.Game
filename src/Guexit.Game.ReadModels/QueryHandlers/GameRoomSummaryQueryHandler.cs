using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameRoomSummaryQuery : IQuery<GameSummaryReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameRoomSummaryQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class GameRoomSummaryQueryHandler : QueryHandler<GameRoomSummaryQuery, GameSummaryReadModel>
{
    public GameRoomSummaryQueryHandler(GameDbContext dbContext, ILogger<QueryHandler<GameRoomSummaryQuery, GameSummaryReadModel>> logger) 
        : base(dbContext, logger)
    { }

    protected override async Task<GameSummaryReadModel> Process(GameRoomSummaryQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking().AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == query.GameRoomId, ct);
        
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var players = await DbContext.Players.AsNoTracking()
            .Where(x => gameRoom.PlayerIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, ct);

        return new GameSummaryReadModel
        {
            GameRoomId = gameRoom.Id,
            Scores = GetScores(gameRoom, players),
            RoundSummaries = GetRoundSummaries(gameRoom, players),
            Status = gameRoom.Status.Value
        };
    }

    private static GameSummaryReadModel.RoundSummaryDto[] GetRoundSummaries(GameRoom gameRoom, Dictionary<PlayerId, Player> players)
    {
        var result = new List<GameSummaryReadModel.RoundSummaryDto>(gameRoom.FinishedRounds.Count);
        foreach (var round in gameRoom.FinishedRounds)
        {
            result.Add(new GameSummaryReadModel.RoundSummaryDto
            {
                FinishedRoundId = round.Id,
                RoundFinishedAt = round.FinishedAt,
                StoryTeller = new StoryTellerDto
                {
                    PlayerId = round.StoryTeller.PlayerId,
                    Story = round.StoryTeller.Story,
                    Username = players[round.StoryTeller.PlayerId].Username
                },
                Scores = round.Scores.Select(x => new GameSummaryReadModel.RoundSummaryDto.ScoreInRoundDto
                {
                    Player = new PlayerDto
                    {
                        PlayerId = x.PlayerId,
                        Username = players[x.PlayerId].Username
                    },
                    Points = x.Points.Value
                }).ToArray(),
                SubmittedCardSummaries = round.SubmittedCardSnapshots.Select(x => new GameSummaryReadModel.RoundSummaryDto.RoundSummarySubmittedCardSummaryDto
                {
                    CardId = x.Card.Id,
                    CardUrl = x.Card.Url,
                    SubmittedBy = new PlayerDto { PlayerId = x.PlayerId, Username = players[x.PlayerId].Username },
                    Voters = x.Voters.Select(voterId => new PlayerDto
                    {
                        PlayerId = voterId,
                        Username = players[voterId].Username
                    }).ToArray(),
                }).ToArray(),
            });
        }

        return result
            .OrderBy(x => x.RoundFinishedAt)
            .ToArray();
    }

    private static GameSummaryReadModel.PlayerScoreDto[] GetScores(GameRoom gameRoom, Dictionary<PlayerId, Player> players)
    {
        var result = new List<GameSummaryReadModel.PlayerScoreDto>(gameRoom.PlayerIds.Count);
        foreach (var playerId in gameRoom.PlayerIds)
        {
            var points = gameRoom.FinishedRounds.Select(x => x.Scores.First(y => y.PlayerId == playerId).Points.Value).Sum();
            result.Add(new GameSummaryReadModel.PlayerScoreDto 
            { 
                Player = new PlayerDto 
                { 
                    PlayerId = playerId, 
                    Username = players[playerId].Username 
                },
                Points = points
            });
        }
        return result.OrderByDescending(x => x.Points).ToArray();
    }
}

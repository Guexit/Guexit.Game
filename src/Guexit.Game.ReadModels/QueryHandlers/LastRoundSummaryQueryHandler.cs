using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.Exceptions;
using Guexit.Game.ReadModels.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class LastRoundSummaryQuery : IQuery<RoundSummaryReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public LastRoundSummaryQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class LastRoundSummaryQueryHandler : QueryHandler<LastRoundSummaryQuery, RoundSummaryReadModel>
{
    public LastRoundSummaryQueryHandler(GameDbContext dbContext, ILogger<LastRoundSummaryQueryHandler> logger)
        : base(dbContext, logger)
    { }

    protected override async Task<RoundSummaryReadModel> Process(LastRoundSummaryQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking()
            .Include(x => x.FinishedRounds).ThenInclude(x => x.Scores)
            .Include(x => x.FinishedRounds).ThenInclude(x => x.SubmittedCardSnapshots).ThenInclude(x => x.Card)
            .Include(x => x.SubmittedCards).ThenInclude(x => x.Card)
            .FirstOrDefaultAsync(x => x.Id == query.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(query.GameRoomId);

        var players = await DbContext.Players.AsNoTracking()
            .Where(x => gameRoom.PlayerIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, ct);

        var lastFinishedRound = gameRoom.FinishedRounds.MaxBy(x => x.FinishedAt)
            ?? throw new CannotReadLastFinishedRoundSummaryIfHasNotAnyFinishedRound(query.GameRoomId);
        
        return new RoundSummaryReadModel
        {
            GameRoomId = query.GameRoomId.Value,
            FinishedRoundId = lastFinishedRound.Id,
            RoundFinishedAt = lastFinishedRound.FinishedAt,
            StoryTeller = new StoryTellerDto
            {
                PlayerId = lastFinishedRound.StoryTeller.PlayerId,
                Story = lastFinishedRound.StoryTeller.Story,
                Username = players[lastFinishedRound.StoryTeller.PlayerId].Username
            },
            Scores = lastFinishedRound.Scores.Select(x => new RoundSummaryReadModel.ScoreDto
            {
                Player = new PlayerDto 
                { 
                    PlayerId = x.PlayerId, 
                    Username = players[x.PlayerId].Username 
                },
                Points = x.Points.Value
            }).ToArray(),
            SubmittedCardSummaries = lastFinishedRound.SubmittedCardSnapshots.Select(x => new RoundSummaryReadModel.SubmittedCardSummaryDto
            { 
                CardId = x.Card.Id,
                CardUrl = x.Card.Url,
                SubmittedBy = new PlayerDto { PlayerId = x.PlayerId, Username = players[x.PlayerId].Username },
                Voters = x.Voters.Select(playerId => new PlayerDto 
                { 
                    PlayerId = playerId, 
                    Username = players[x.PlayerId].Username 
                }).ToArray(),
            }).ToArray(),
        };
    }
}

using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.Exceptions;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;
using Mediator;
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

public sealed class LastRoundSummaryQueryHandler : IQueryHandler<LastRoundSummaryQuery, RoundSummaryReadModel>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;
    private readonly ReadOnlyPlayersRepository _playersRepository;

    public LastRoundSummaryQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository, ReadOnlyPlayersRepository playersRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _playersRepository = playersRepository;
    }

    public async ValueTask<RoundSummaryReadModel> Handle(LastRoundSummaryQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct);
        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var players = (await _playersRepository.GetBy(gameRoom.PlayerIds, ct)).ToDictionary(x => x.Id);

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
                Username = players[lastFinishedRound.StoryTeller.PlayerId].Username,
                Nickname = players[lastFinishedRound.StoryTeller.PlayerId].Nickname.Value
            },
            Scores = lastFinishedRound.Scores.OrderByDescending(x => x.Points).Select(x => new RoundSummaryReadModel.ScoreDto
            {
                Player = new PlayerDto 
                { 
                    PlayerId = x.PlayerId, 
                    Username = players[x.PlayerId].Username,
                    Nickname = players[x.PlayerId].Nickname.Value
                },
                Points = x.Points.Value
            }).ToArray(),
            SubmittedCardSummaries = lastFinishedRound.SubmittedCardSnapshots.Select(x => new RoundSummaryReadModel.SubmittedCardSummaryDto
            { 
                CardId = x.Card.Id,
                CardUrl = x.Card.Url,
                SubmittedBy = new PlayerDto
                {
                    PlayerId = x.PlayerId, 
                    Username = players[x.PlayerId].Username,
                    Nickname = players[x.PlayerId].Nickname.Value,
                },
                Voters = x.Voters.Select(voterId => new PlayerDto 
                { 
                    PlayerId = voterId, 
                    Username = players[voterId].Username,
                    Nickname = players[voterId].Nickname.Value
                }).ToArray(),
            }).ToArray(),
        };
    }
}

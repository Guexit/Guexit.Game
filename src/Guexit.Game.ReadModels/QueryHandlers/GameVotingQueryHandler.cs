using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Persistence;
using Guexit.Game.ReadModels.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameVotingQuery : IQuery<VotingReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameVotingQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class GameVotingQueryHandler : QueryHandler<GameVotingQuery, VotingReadModel>
{
    public GameVotingQueryHandler(GameDbContext dbContext, ILogger<GameVotingQueryHandler> logger)
        : base(dbContext, logger)
    { }

    protected override async Task<VotingReadModel> Process(GameVotingQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking()
            .Include(x => x.SubmittedCards).ThenInclude(x => x.Card)
            .SingleOrDefaultAsync(x => x.Id == query.GameRoomId, cancellationToken: ct);

        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        var allVoters = gameRoom.SubmittedCards.SelectMany(x => x.Voters).ToHashSet();
        var players = await DbContext.Players.AsNoTracking()
            .Where(x => allVoters.Contains(x.Id))
            .ToArrayAsync(ct);

        var submittedCards = gameRoom.SubmittedCards
            .Select(x => new VotingReadModel.CardDto { Id = x.Card.Id, Url = x.Card.Url })
            .ToArray();
        var playersWhoVoted = players
            .Select(x => new VotingReadModel.PlayerDto() { PlayerId = x.Id, Username = x.Username })
            .ToArray();

        return new VotingReadModel
        {
            Cards = submittedCards,
            PlayersWhoHaveAlreadyVoted = playersWhoVoted,
            CurrentUserHasAlreadyVoted = allVoters.Contains(query.PlayerId),
            IsCurrentUserStoryTeller = gameRoom.CurrentStoryTeller.PlayerId == query.PlayerId,
        };
    }
}

using Guexit.Game.Persistence;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.ReadModels.Exceptions;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameBoardQuery : IQuery<GameBoardReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameBoardQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new(gameRoomId);
        PlayerId = new(playerId);
    }
}

public sealed class GameBoardQueryHandler : QueryHandler<GameBoardQuery, GameBoardReadModel>
{
    public GameBoardQueryHandler(GameDbContext dbContext, ILogger<GameBoardQueryHandler> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<GameBoardReadModel> Process(GameBoardQuery query, CancellationToken ct)
    {
        var gameRoom = await DbContext.GameRooms.AsNoTracking()
            .Include(x => x.Deck)
            .Include(x => x.PlayerHands).ThenInclude(x => x.Cards)
            .Include(x => x.SubmittedCards).ThenInclude(x => x.Card)
            .SingleOrDefaultAsync(x => x.Id == query.GameRoomId, ct);

        if (gameRoom is null) 
            throw new GameRoomNotFoundException(query.GameRoomId);

        if (gameRoom.Status != GameStatus.InProgress)
            throw new CannotReadBoardIfGameIsNotInProgressException(gameRoom.Id, gameRoom.Status);

        var currentStoryTeller = await DbContext.Players.AsNoTracking()
            .SingleAsync(x => x.Id == gameRoom.CurrentStoryTeller.PlayerId, ct);

        var allSubmittedCards = gameRoom.SubmittedCards.Select(x => new GameBoardReadModel.CardDto { Id = x.Card.Id, Url = x.Card.Url }).ToArray();
        var submittedCardOfCurrentUser = gameRoom.SubmittedCards.FirstOrDefault(x => x.PlayerId == query.PlayerId);

        var readModel = new GameBoardReadModel()
        {
            GameRoomId = gameRoom.Id,
            CurrentStoryTeller = new GameBoardReadModel.StoryTellerDto 
            { 
                PlayerId = currentStoryTeller.Id.Value, 
                Username = currentStoryTeller.Username, 
                Story = gameRoom.CurrentStoryTeller.Story
            },
            PlayerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == query.PlayerId).Cards.Select(x => new GameBoardReadModel.CardDto
            {
                Id = x.Id,
                Url = x.Url
            }).ToArray(),
            IsCurrentUserStoryTeller = query.PlayerId == gameRoom.CurrentStoryTeller.PlayerId,
            CurrentUserSubmittedCard = submittedCardOfCurrentUser is null 
                ? null 
                : new GameBoardReadModel.CardDto { Id = submittedCardOfCurrentUser.Card.Id, Url = submittedCardOfCurrentUser.Card.Url },
            SubmittedCards = allSubmittedCards
        };
        return readModel;
    }
}

using Guexit.Game.Persistence;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.ReadModels.Exceptions;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class GameBoardQuery : IQuery<BoardReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public GameBoardQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new(gameRoomId);
        PlayerId = new(playerId);
    }
}

public sealed class GameBoardQueryHandler : QueryHandler<GameBoardQuery, BoardReadModel>
{
    public GameBoardQueryHandler(GameDbContext dbContext, ILogger<GameBoardQueryHandler> logger) 
        : base(dbContext, logger)
    {
    }

    protected override async Task<BoardReadModel> Process(GameBoardQuery query, CancellationToken ct)
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

        var playersInGameRoom = await DbContext.Players.AsNoTracking()
            .Where(x => gameRoom.PlayerIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, ct);

        var currentUserSubmittedCard = GetCurrentUserSubmittedCard(gameRoom, query.PlayerId);
        var guessingPlayers = GetGuessingPlayers(gameRoom, playersInGameRoom);
        var currentUserPlayerHand = GetCurrentUserPlayerHand(gameRoom, query.PlayerId);

        var readModel = new BoardReadModel
        {
            GameRoomId = gameRoom.Id,
            CurrentStoryTeller = new StoryTellerDto
            {
                PlayerId = gameRoom.CurrentStoryTeller.PlayerId.Value,
                Username = playersInGameRoom[gameRoom.CurrentStoryTeller.PlayerId].Username,
                Story = gameRoom.CurrentStoryTeller.Story
            },
            PlayerHand = currentUserPlayerHand,
            IsCurrentUserStoryTeller = query.PlayerId == gameRoom.CurrentStoryTeller.PlayerId,
            CurrentUserSubmittedCard = currentUserSubmittedCard,
            GuessingPlayers = guessingPlayers
        };
        return readModel;
    }

    private static BoardReadModel.CardDto? GetCurrentUserSubmittedCard(GameRoom gameRoom, PlayerId currentPlayerId)
    {
        var submittedCardOfCurrentUser = gameRoom.SubmittedCards.FirstOrDefault(x => x.PlayerId == currentPlayerId);
        if (submittedCardOfCurrentUser is null)
            return null;

        return new BoardReadModel.CardDto 
        {
            Id = submittedCardOfCurrentUser.Card.Id,
            Url = submittedCardOfCurrentUser.Card.Url
        };
    }

    private static BoardReadModel.GuessingPlayersDto[] GetGuessingPlayers(GameRoom gameRoom, Dictionary<PlayerId, Player> playersInGameRoom)
    {
        var submittedCardsByPlayerId = gameRoom.SubmittedCards.ToDictionary(x => x.PlayerId);

        var guessingPlayersDtos = gameRoom.GetCurrentGuessingPlayerIds().Select(playerId => new BoardReadModel.GuessingPlayersDto
        {
            HasSubmittedCardAlready = submittedCardsByPlayerId.ContainsKey(playerId),
            PlayerId = playerId,
            Username = playersInGameRoom[playerId].Username
        }).ToArray();

        return guessingPlayersDtos;
    }

    private static BoardReadModel.CardDto[] GetCurrentUserPlayerHand(GameRoom gameRoom, PlayerId currentPlayerId)
    {
        var playerHand = gameRoom.PlayerHands.Single(x => x.PlayerId == currentPlayerId);

        var cardDtos = playerHand.Cards.Select(x => new BoardReadModel.CardDto
        {
            Id = x.Id,
            Url = x.Url
        }).ToArray();

        return cardDtos;
    }
}

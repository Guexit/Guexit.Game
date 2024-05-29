using Guexit.Game.Persistence;
using Microsoft.Extensions.Logging;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Microsoft.EntityFrameworkCore;
using Guexit.Game.Application.Exceptions;
using Guexit.Game.ReadModels.Exceptions;
using Guexit.Game.ReadModels.ReadOnlyRepositories;

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

public sealed class GameBoardQueryHandler : IQueryHandler<GameBoardQuery, BoardReadModel>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;
    private readonly ReadOnlyPlayersRepository _playersRepository;

    public GameBoardQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository, ReadOnlyPlayersRepository playersRepository)
    {
        _gameRoomRepository = gameRoomRepository;
        _playersRepository = playersRepository;
    }

    public async ValueTask<BoardReadModel> Handle(GameBoardQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct);

        if (gameRoom is null)
            throw new GameRoomNotFoundException(query.GameRoomId);

        if (gameRoom.Status != GameStatus.InProgress)
            throw new CannotReadBoardIfGameIsNotInProgressException(gameRoom.Id, gameRoom.Status);

        var playersInGameRoom = (await _playersRepository.GetBy(gameRoom.PlayerIds, ct)).ToDictionary(x => x.Id);

        var currentUserSubmittedCard = GetCurrentUserSubmittedCard(gameRoom, query.PlayerId);
        var guessingPlayers = GetGuessingPlayers(gameRoom, playersInGameRoom);
        var currentUserPlayerHand = GetCurrentUserPlayerHand(gameRoom, query.PlayerId);
        var currentPlayerCardReRollState = GetCurrentPlayerCardReRollState(gameRoom, query);

        var readModel = new BoardReadModel
        {
            GameRoomId = gameRoom.Id,
            CurrentStoryTeller = new StoryTellerDto
            {
                PlayerId = gameRoom.CurrentStoryTeller.PlayerId.Value,
                Username = playersInGameRoom[gameRoom.CurrentStoryTeller.PlayerId].Username,
                Nickname = playersInGameRoom[gameRoom.CurrentStoryTeller.PlayerId].Nickname.Value,
                Story = gameRoom.CurrentStoryTeller.Story
            },
            PlayerHand = currentUserPlayerHand,
            IsCurrentUserStoryTeller = query.PlayerId == gameRoom.CurrentStoryTeller.PlayerId,
            CurrentUserSubmittedCard = currentUserSubmittedCard,
            GuessingPlayers = guessingPlayers,
            CurrentPlayer = new PlayerDto
            {
                PlayerId = query.PlayerId.Value,
                Nickname = playersInGameRoom[query.PlayerId].Nickname.Value,
                Username = playersInGameRoom[query.PlayerId].Username,
            },
            CurrentPlayerCardReRollState = currentPlayerCardReRollState.Value
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
        var playerIdsThatSubmittedTheCardAlready = gameRoom.SubmittedCards.Select(x => x.PlayerId).ToHashSet();

        var guessingPlayersDtos = gameRoom.GetCurrentGuessingPlayerIds().Select(playerId => new BoardReadModel.GuessingPlayersDto
        {
            HasSubmittedCardAlready = playerIdsThatSubmittedTheCardAlready.Contains(playerId),
            PlayerId = playerId,
            Username = playersInGameRoom[playerId].Username,
            Nickname = playersInGameRoom[playerId].Nickname.Value
        }).ToArray();

        return guessingPlayersDtos;
    }

    private static BoardReadModel.CardDto[] GetCurrentUserPlayerHand(GameRoom gameRoom, PlayerId currentPlayerId)
    {
        var playerHand = gameRoom.PlayerHands.First(x => x.PlayerId == currentPlayerId);

        var cardDtos = playerHand.Cards.OrderBy(x => x.Id)
            .Select(x => new BoardReadModel.CardDto
            {
                Id = x.Id,
                Url = x.Url
            }).ToArray();

        return cardDtos;
    }

    private static CardReRollState GetCurrentPlayerCardReRollState(GameRoom gameRoom, GameBoardQuery query)
    {
        var currentPlayerCardReRoll = gameRoom.CurrentCardReRolls.FirstOrDefault(x => x.PlayerId == query.PlayerId);
        return CardReRollState.From(currentPlayerCardReRoll);
    }
}

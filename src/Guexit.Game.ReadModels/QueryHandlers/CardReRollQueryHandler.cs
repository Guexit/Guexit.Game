using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.ReadModels.Exceptions;
using Guexit.Game.ReadModels.ReadModels;
using Guexit.Game.ReadModels.ReadOnlyRepositories;

namespace Guexit.Game.ReadModels.QueryHandlers;

public sealed class CardReRollQuery : IQuery<CardReRollReadModel>
{
    public GameRoomId GameRoomId { get; }
    public PlayerId PlayerId { get; }

    public CardReRollQuery(Guid gameRoomId, string playerId)
    {
        GameRoomId = new GameRoomId(gameRoomId);
        PlayerId = new PlayerId(playerId);
    }
}

public sealed class CardReRollQueryHandler : IQueryHandler<CardReRollQuery, CardReRollReadModel>
{
    private readonly ReadOnlyGameRoomRepository _gameRoomRepository;

    public CardReRollQueryHandler(ReadOnlyGameRoomRepository gameRoomRepository)
    {
        _gameRoomRepository = gameRoomRepository;
    }

    public async ValueTask<CardReRollReadModel> Handle(CardReRollQuery query, CancellationToken ct)
    {
        var gameRoom = await _gameRoomRepository.GetBy(query.GameRoomId, ct)
            ?? throw new GameRoomNotFoundException(query.GameRoomId);

        var cardReRoll = gameRoom.CurrentCardReRolls.FirstOrDefault(x => x.PlayerId == query.PlayerId)
            ?? throw CardsForReRollUnavailableException.NotReserved(query.GameRoomId, query.PlayerId);

        if (cardReRoll.IsCompleted)
            throw CardsForReRollUnavailableException.AlreadyCompleted(query.GameRoomId, query.PlayerId);

        var playerHand = gameRoom.PlayerHands.First(x => x.PlayerId == query.PlayerId);

        return new CardReRollReadModel
        {
            GameRoomId = gameRoom.Id.Value,
            CurrentPlayerHand = playerHand.Cards.Select(x => new CardReRollReadModel.CardForReRollDto 
            {
                Id = x.Id,
                Url = x.Url,
            }).ToArray(),
            ReservedCardsToReRoll = cardReRoll.ReservedCards.Select(x => new CardReRollReadModel.CardForReRollDto
            {
                Id = x.Id,
                Url = x.Url,
            }).ToArray()
        };
    }
}

using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : AggregateRoot<GameRoomId>
{
    private const int TotalCardsPerPlayer = 8;
    private const int CardsInHandPerPlayer = 4;

    public ICollection<PlayerId> PlayerIds { get; private set; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    public ICollection<Card> Deck { get; private set; } = new List<Card>();
    public ICollection<PlayerHand> PlayerHands { get; private set; } = new List<PlayerHand>();

    private GameRoom()
    {
        // Entity Framework required parameterless ctor
    }

    public GameRoom(GameRoomId id, PlayerId creatorId, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        PlayerIds.Add(creatorId);
    }

    public void DefineMinRequiredPlayers(int count)
    {
        RequiredMinPlayers = new RequiredMinPlayers(count);
    }

    public void Join(PlayerId playerId)
    {
        if (PlayerIds.Contains(playerId))
            throw new PlayerIsAlreadyInGameRoomException(playerId);

        PlayerIds.Add(playerId);

        AddDomainEvent(new PlayerJoinedGameRoom(Id, playerId));
    }

    public void Start()
    {
        if (!RequiredMinPlayers.IsSatisfiedBy(PlayerIds.Count))
            throw new InsufficientPlayersToStartGameException(Id, PlayerIds.Count, RequiredMinPlayers);

        Status = GameStatus.AssigningCards;

        AddDomainEvent(new GameStarted(Id));
    }

    public int GetRequiredNumberOfCardsInDeck() => PlayerIds.Count * TotalCardsPerPlayer;

    public void AssignDeck(IEnumerable<Card> cards)
    {
        Deck = new List<Card>(cards);
        Status = GameStatus.InProgress;
        DispatchInitialPlayerHands();

        AddDomainEvent(new DeckAssigned(Id));
    }

    private void DispatchInitialPlayerHands()
    {
        foreach (var player in PlayerIds)
        {
            var cardsToDispatch = new List<Card>(CardsInHandPerPlayer);
            for (int i = 0; i < CardsInHandPerPlayer; i++)
            {
                var card = Deck.Last();
                cardsToDispatch.Add(card);
                Deck.Remove(card);
            }

            PlayerHands.Add(new PlayerHand(Guid.NewGuid(), player, cardsToDispatch, Id));
        }
    }
}

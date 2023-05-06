using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : AggregateRoot<GameRoomId>
{
    public const int TotalCardsPerPlayer = 8;
    public const int CardsInHandPerPlayer = 4;

    public ICollection<PlayerId> PlayerIds { get; private set; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    public ICollection<Card> Deck { get; private set; } = new List<Card>();
    public ICollection<PlayerHand> PlayerHands { get; private set; } = new List<PlayerHand>();
    public StoryTeller CurrentStoryTeller { get; private set; } = StoryTeller.Empty;

    private PlayerHand CurrentStoryTellerHand => PlayerHands.Single(x => x.PlayerId == CurrentStoryTeller.PlayerId);

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

    public void DefineMinRequiredPlayers(int count) => RequiredMinPlayers = new RequiredMinPlayers(count);

    public void Join(PlayerId playerId)
    {
        if (Status != GameStatus.NotStarted)
            throw new CannotJoinStartedGameException(playerId, Id);

        if (PlayerIds.Contains(playerId))
            throw new PlayerIsAlreadyInGameRoomException(playerId);

        PlayerIds.Add(playerId);

        AddDomainEvent(new PlayerJoined(Id, playerId));
    }

    public void Start()
    {
        if (Status != GameStatus.NotStarted)
            throw new CannotStartAlreadyStartedGameException(Id);

        if (!RequiredMinPlayers.AreSatisfiedBy(PlayerIds.Count))
            throw new InsufficientPlayersToStartGameException(Id, PlayerIds.Count, RequiredMinPlayers);

        Status = GameStatus.AssigningCards;
        CurrentStoryTeller = StoryTeller.Create(PlayerIds.First());

        AddDomainEvent(new GameStarted(Id));
    }

    public int GetRequiredNumberOfCardsInDeck() => PlayerIds.Count * TotalCardsPerPlayer;

    public void AssignDeck(IEnumerable<Card> cards)
    {
        if (cards.Count() < GetRequiredNumberOfCardsInDeck())
            throw new InsufficientImagesToAssignDeckException(cards.Count(), Id);

        Deck = new List<Card>(cards);
        DealInitialPlayerHands();
        Status = GameStatus.InProgress;

        AddDomainEvents(new DeckAssigned(Id), new InitialCardsDealed(Id));
    }

    public void SubmitCardStory(PlayerId storyTellerId, CardId cardId, string story)
    {
        if (Status != GameStatus.InProgress)
            throw new CannotSubmitCardStoryIfGameRoomIsNotInProgressException(Id);

        if (CurrentStoryTeller.PlayerId != storyTellerId)
            throw new CannotSubmitCardStoryIfPlayerIsNotCurrentStoryTellerException(Id, storyTellerId);

        if (CurrentStoryTeller.HasSubmittedCardStory())
            throw new CardStoryAlreadySubmittedException(Id, storyTellerId);

        var card = CurrentStoryTellerHand.Cards.SingleOrDefault(x => x.Id == cardId) 
            ?? throw new CardNotFoundInPlayerHandException(Id, storyTellerId, cardId);

        CurrentStoryTeller = CurrentStoryTeller.SubmitCardWithStory(card, story);

        AddDomainEvent(new CardStorySubmitted(Id, storyTellerId, cardId, story));
    }

    private void DealInitialPlayerHands()
    {
        foreach (var player in PlayerIds)
        {
            var cardsToDeal = new List<Card>(CardsInHandPerPlayer);
            for (int i = 0; i < CardsInHandPerPlayer; i++)
            {
                var card = Deck.First();
                cardsToDeal.Add(card);
                Deck.Remove(card);
            }

            PlayerHands.Add(new PlayerHand(Guid.NewGuid(), player, cardsToDeal, Id));
        }
    }
}

public sealed class GameRoomId : ValueObject
{
    public static readonly GameRoomId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public GameRoomId(Guid value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator GameRoomId(Guid value) => new(value);
    public static implicit operator Guid(GameRoomId value) => value.Value;
}

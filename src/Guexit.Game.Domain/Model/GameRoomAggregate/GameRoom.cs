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
    public ICollection<SubmittedCard> SubmittedCards { get; private set; } = new List<SubmittedCard>();
    public StoryTeller CurrentStoryTeller { get; private set; } = StoryTeller.Empty;

    public ICollection<FinishedRound> FinishedRounds { get; private set; } = new List<FinishedRound>();

    private PlayerHand CurrentStoryTellerHand => PlayerHands.Single(x => x.PlayerId == CurrentStoryTeller.PlayerId);
    private HashSet<PlayerId> CurrentGuessingPlayerIds => PlayerIds.Where(x => x != CurrentStoryTeller.PlayerId).ToHashSet();

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
            throw new JoinStartedGameException(playerId, Id);

        if (PlayerIds.Contains(playerId))
            throw new PlayerIsAlreadyInGameRoomException(playerId);

        PlayerIds.Add(playerId);

        AddDomainEvent(new PlayerJoined(Id, playerId));
    }

    public void Start()
    {
        if (Status != GameStatus.NotStarted)
            throw new StartAlreadyStartedGameException(Id);

        if (!RequiredMinPlayers.AreSatisfiedBy(PlayerIds.Count))
            throw new InsufficientPlayersToStartGameException(Id, PlayerIds.Count, RequiredMinPlayers);

        Status = GameStatus.AssigningCards;
        CurrentStoryTeller = StoryTeller.Create(PlayerIds.First());

        AddDomainEvent(new GameStarted(Id));
    }

    public int GetRequiredNumberOfCardsInDeck() => PlayerIds.Count * TotalCardsPerPlayer;

    public void AssignDeck(Card[] cards)
    {
        if (cards.Length < GetRequiredNumberOfCardsInDeck())
            throw new InsufficientImagesToAssignDeckException(cards.Length, Id);

        Deck = new List<Card>(cards);
        DealInitialPlayerHands();
        Status = GameStatus.InProgress;

        AddDomainEvents(new DeckAssigned(Id), new InitialCardsDealed(Id));
    }

    public void SubmitStoryTellerCardStory(PlayerId storyTellerId, CardId cardId, string story)
    {
        if (Status != GameStatus.InProgress)
            throw new SubmittingCardToGameNotInProgressException(Id);

        if (CurrentStoryTeller.PlayerId != storyTellerId)
            throw new InvalidCardStorySubmissionForNonStoryTellerException(Id, storyTellerId);

        if (CurrentStoryTeller.HasSubmittedCardStory())
            throw new StoryAlreadySubmittedException(Id, storyTellerId);

        var card = CurrentStoryTellerHand.SubstractCard(cardId);
        SubmittedCards.Add(new SubmittedCard(storyTellerId, card, Id));
        CurrentStoryTeller = CurrentStoryTeller.SubmitStory(story);

        AddDomainEvent(new StoryTellerCardStorySubmitted(Id, storyTellerId, cardId, story));
    }

    public void SubmitGuessingPlayerCard(PlayerId guessingPlayerId, CardId cardId)
    {
        if (Status != GameStatus.InProgress)
            throw new SubmittingCardToGameNotInProgressException(Id);

        EnsureCurrentGuessingPlayersContains(guessingPlayerId);

        if (!CurrentStoryTeller.HasSubmittedCardStory())
            throw new GuessingPlayerCannotSubmitCardIfStoryTellerHaveNotSubmitStoryException(Id, guessingPlayerId);

        var playerHand = PlayerHands.Single(x => x.PlayerId == guessingPlayerId);

        var card = playerHand.SubstractCard(cardId);
        SubmittedCards.Add(new SubmittedCard(guessingPlayerId, card, Id));
        AddDomainEvent(new GuessingPlayerCardSubmitted(Id, guessingPlayerId, cardId));

        var allPlayersHaveSubmittedTheirCards = SubmittedCards.Count == PlayerIds.Count;
        if (allPlayersHaveSubmittedTheirCards)
            AddDomainEvent(new AllPlayerCardsSubmitted(Id));
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

    public void VoteCard(PlayerId votingPlayerId, CardId submittedCardId)
    {
        if (Status != GameStatus.InProgress)
            throw new VoteCardToNotInProgressGameRoomException(Id);

        if (SubmittedCards.Count < PlayerIds.Count)
            throw new CannotVoteIfAnyPlayerIsPendingToSubmitCardException(Id);
        
        EnsureCurrentGuessingPlayersContains(votingPlayerId);

        var voters = SubmittedCards.SelectMany(x => x.Voters).ToHashSet();
        if (voters.Contains(votingPlayerId))
            throw new PlayerAlreadyVotedException(Id, votingPlayerId);
        
        var submittedCard = SubmittedCards.SingleOrDefault(x => x.Card.Id == submittedCardId)
            ?? throw new CardNotFoundInSubmittedCardsException(Id, submittedCardId);

        submittedCard.Vote(votingPlayerId);

        var allPlayersHaveVoted = SubmittedCards.SelectMany(x => x.Voters).Count() == CurrentGuessingPlayerIds.Count;
        if (allPlayersHaveVoted)
            ComputeVotingScoreAndFinishRound();
    }

    private void EnsureCurrentGuessingPlayersContains(PlayerId playerId)
    {
        if (!CurrentGuessingPlayerIds.Contains(playerId))
            throw new PlayerNotFoundInCurrentGuessingPlayersException(playerId);
    }

    private void ComputeVotingScoreAndFinishRound()
    {
        var votingScore = PlayerIds.ToDictionary(x => x, v => Points.Zero);

        var submittedCardsByPlayerId = SubmittedCards.ToDictionary(x => x.PlayerId, v => v);

        var playerCountWhoVotedStoryTellerCard = submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Count;
        if (playerCountWhoVotedStoryTellerCard > 0 && playerCountWhoVotedStoryTellerCard < CurrentGuessingPlayerIds.Count)
            votingScore[CurrentStoryTeller.PlayerId] += new Points(3);

        foreach (var guessingPlayerId in CurrentGuessingPlayerIds)
        {
            if (submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Contains(guessingPlayerId))
                votingScore[guessingPlayerId] += new Points(3);

            votingScore[guessingPlayerId] += new Points(submittedCardsByPlayerId[guessingPlayerId].Voters.Count);
        }

        AddDomainEvent(new VotingScoresComputed(Id));
        
        FinishedRounds.Add(new FinishedRound(Id, DateTimeOffset.UtcNow, votingScore.AsReadOnly(), SubmittedCards.ToArray(), CurrentStoryTeller));
        ShiftTurn();
    }

    private void ShiftTurn()
    {
        var playerIds = PlayerIds.ToList();

        var currentStoryTellerIndex = playerIds.IndexOf(CurrentStoryTeller.PlayerId);
        var nextStoryTellerIndex = (currentStoryTellerIndex + 1) % playerIds.Count;

        CurrentStoryTeller = StoryTeller.Create(playerIds[nextStoryTellerIndex]);

        foreach (var player in PlayerIds)
        {
            var card = Deck.First();
            PlayerHands.Single(x => x.PlayerId == player).AddCard(card);
            Deck.Remove(card);
        }

        SubmittedCards.Clear();
    }
}

public sealed class GameRoomId : ValueObject
{
    public static readonly GameRoomId Empty = new(Guid.Empty);

    public Guid Value { get; }

    public GameRoomId(Guid value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator GameRoomId(Guid value) => new(value);
    public static implicit operator Guid(GameRoomId value) => value.Value;
}

using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : AggregateRoot<GameRoomId>
{
    public const int TotalCardsPerPlayer = 8;
    public const int PlayerHandSize = 4;

    public ICollection<PlayerId> PlayerIds { get; private set; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    public ICollection<Card> Deck { get; private set; } = new List<Card>();
    public ICollection<PlayerHand> PlayerHands { get; private set; } = new List<PlayerHand>();
    public ICollection<SubmittedCard> SubmittedCards { get; private set; } = new List<SubmittedCard>();
    public StoryTeller CurrentStoryTeller { get; private set; } = StoryTeller.Empty;
    public ICollection<FinishedRound> FinishedRounds { get; private set; } = new List<FinishedRound>();

    public IReadOnlySet<PlayerId> GetCurrentGuessingPlayerIds() => PlayerIds.Where(x => x != CurrentStoryTeller.PlayerId).ToHashSet();
    public int GetPlayersCount() => PlayerIds.Count;

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

    public int GetRequiredNumberOfCardsInDeck() => PlayerIds.Count * TotalCardsPerPlayer;
    public void DefineMinRequiredPlayers(int count) => RequiredMinPlayers = new RequiredMinPlayers(count);

    public void Join(PlayerId playerId)
    {
        if (PlayerIds.Contains(playerId))
            return;

        if (Status != GameStatus.NotStarted)
            throw new JoinStartedGameException(playerId, Id);

        PlayerIds.Add(playerId);

        AddDomainEvent(new PlayerJoined(Id, playerId));
    }

    public void AssignDeck(Card[] cards)
    {
        if (cards.Length < GetRequiredNumberOfCardsInDeck())
            throw new InsufficientImagesToAssignDeckException(cards.Length, Id);

        Deck = new List<Card>(cards);
    }

    public void Start(PlayerId playerId)
    {
        if (Status != GameStatus.NotStarted)
            throw new StartAlreadyStartedGameException(Id);

        if (!RequiredMinPlayers.AreSatisfiedBy(PlayerIds.Count))
            throw new InsufficientPlayersToStartGameException(Id, PlayerIds.Count, RequiredMinPlayers);
        
        if (!PlayerIds.Contains(playerId))
            throw new PlayerNotInGameRoomException(Id, playerId);

        DealInitialPlayerHands();
        
        CurrentStoryTeller = StoryTeller.Create(PlayerIds.First());
        Status = GameStatus.InProgress;
        
        AddDomainEvent(new GameStarted(Id));
    }

    public void SubmitStory(PlayerId storyTellerId, CardId cardId, string story)
    {
        if (Status != GameStatus.InProgress)
            throw new SubmittingCardToGameNotInProgressException(Id);

        if (CurrentStoryTeller.PlayerId != storyTellerId)
            throw new InvalidCardStorySubmissionForNonStoryTellerException(Id, storyTellerId);

        if (CurrentStoryTeller.HasSubmittedCardStory())
            throw new StoryAlreadySubmittedException(Id, storyTellerId);

        var card = GetCurrentStoryTellerHand().SubstractCard(cardId);
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
            var cardsToDeal = new List<Card>(PlayerHandSize);
            for (int i = 0; i < PlayerHandSize; i++)
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

        var playersWhoAlreadyVoted = SubmittedCards.SelectMany(x => x.Voters).ToHashSet();
        if (playersWhoAlreadyVoted.Contains(votingPlayerId))
            throw new PlayerAlreadyVotedException(Id, votingPlayerId);

        var submittedCard = SubmittedCards.SingleOrDefault(x => x.Card.Id == submittedCardId)
            ?? throw new CardNotFoundInSubmittedCardsException(Id, submittedCardId);

        submittedCard.Vote(votingPlayerId);

        if (AllPlayersHaveVoted())
            CompleteCurrentRound();
    }

    private PlayerHand GetCurrentStoryTellerHand() => PlayerHands.Single(x => x.PlayerId == CurrentStoryTeller.PlayerId);

    private bool AllPlayersHaveVoted() => SubmittedCards.SelectMany(x => x.Voters).Count() == GetCurrentGuessingPlayerIds().Count;

    private void EnsureCurrentGuessingPlayersContains(PlayerId playerId)
    {
        if (!GetCurrentGuessingPlayerIds().Contains(playerId))
            throw new PlayerNotFoundInCurrentGuessingPlayersException(playerId);
    }

    private void CompleteCurrentRound()
    {
        var submittedCardsByPlayerId = SubmittedCards.ToDictionary(x => x.PlayerId);
        var amountOfPlayersWhoVotedStoryTellerCard = submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Count;

        var pointsByPlayer = PlayerIds.ToDictionary(x => x, v => Points.Zero);
        if (amountOfPlayersWhoVotedStoryTellerCard > 0 && amountOfPlayersWhoVotedStoryTellerCard < GetCurrentGuessingPlayerIds().Count)
            pointsByPlayer[CurrentStoryTeller.PlayerId] += new Points(3);

        foreach (var guessingPlayerId in GetCurrentGuessingPlayerIds())
        {
            if (submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Contains(guessingPlayerId))
                pointsByPlayer[guessingPlayerId] += new Points(3);

            pointsByPlayer[guessingPlayerId] += new Points(submittedCardsByPlayerId[guessingPlayerId].Voters.Count);
        }

        AddDomainEvent(new VotingScoresComputed(Id));
        
        FinishedRounds.Add(new FinishedRound(Id, DateTimeOffset.UtcNow, pointsByPlayer.AsReadOnly(), SubmittedCards.ToArray(), CurrentStoryTeller));

        if (Deck.Count >= PlayerIds.Count)
            ShiftToNextRound();
        else
            End();
    }

    private void End()
    {
        Status = GameStatus.Finished;
        AddDomainEvent(new GameFinished(Id));
    }

    private void ShiftToNextRound()
    {
        var playerIds = PlayerIds.ToList();
        var currentStoryTellerIndex = playerIds.IndexOf(CurrentStoryTeller.PlayerId);
        var nextStoryTellerIndex = (currentStoryTellerIndex + 1) % playerIds.Count;
        CurrentStoryTeller = StoryTeller.Create(playerIds[nextStoryTellerIndex]);

        foreach (var playerId in PlayerIds)
        {
            var card = Deck.First();
            PlayerHands.Single(x => x.PlayerId == playerId).AddCard(card);
            Deck.Remove(card);
        }

        SubmittedCards.Clear();
        AddDomainEvent(new NewRoundStarted(Id));
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

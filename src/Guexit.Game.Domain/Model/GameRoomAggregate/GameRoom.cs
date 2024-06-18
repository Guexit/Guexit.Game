using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;
using Guexit.Game.Domain.Services;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : AggregateRoot<GameRoomId>
{
    public const int PlayerHandSize = 4;
    public const int MaximumPlayers = 10;

    public PlayerId CreatedBy { get; private init; } = PlayerId.Empty;
    public ICollection<PlayerId> PlayerIds { get; private init; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    public ICollection<Card> Deck { get; private set; } = new List<Card>();
    public ICollection<PlayerHand> PlayerHands { get; private init; } = new List<PlayerHand>();
    public ICollection<SubmittedCard> SubmittedCards { get; private init; } = new List<SubmittedCard>();
    public StoryTeller CurrentStoryTeller { get; private set; } = StoryTeller.Empty;
    public ICollection<FinishedRound> FinishedRounds { get; private init; } = new List<FinishedRound>();
    public ICollection<CardReRoll> CurrentCardReRolls { get; private init; } = new List<CardReRoll>();
    public GameRoomId NextGameRoomId { get; private set; } = GameRoomId.Empty;
    public bool IsPublic { get; private set; }

    public IReadOnlySet<PlayerId> GetCurrentGuessingPlayerIds() => PlayerIds.Where(x => x != CurrentStoryTeller.PlayerId).ToHashSet();
    public int PlayerCount => PlayerIds.Count;

    private GameRoom()
    {
        // Entity Framework required parameterless ctor
    }

    public GameRoom(GameRoomId id, PlayerId creatorId, DateTimeOffset createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        CreatedBy = creatorId;
        PlayerIds.Add(creatorId);
    }

    public int GetRequiredNumberOfCardsInDeck() => DeckSizeService.Calculate(PlayerCount, desiredRounds: 1);

    public void Join(PlayerId playerId)
    {
        if (PlayerIds.Contains(playerId))
            return;
        
        if (PlayerCount >= MaximumPlayers)
            throw new CannotJoinFullGameRoomException(playerId, Id);

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
        EnsurePlayersContains(playerId);
        
        if (Status != GameStatus.NotStarted)
            throw new StartAlreadyStartedGameException(Id);

        if (!RequiredMinPlayers.IsSatisfiedBy(PlayerIds.Count))
            throw new InsufficientPlayersToStartGameException(Id, PlayerIds.Count, RequiredMinPlayers);

        if (CreatedBy != playerId)
            throw new GamePermissionDeniedException(Id, playerId);
        
        DealInitialPlayerHands();
        
        CurrentStoryTeller = StoryTeller.Create(PlayerIds.First());
        Status = GameStatus.InProgress;
        
        AddDomainEvent(new GameStarted(Id));
    }

    public void SubmitStory(PlayerId storyTellerId, CardId cardId, string story)
    {
        EnsureGameIsInProgress(storyTellerId);
        EnsurePlayersContains(storyTellerId);

        if (CurrentStoryTeller.PlayerId != storyTellerId)
            throw new InvalidCardStorySubmissionForNonStoryTellerException(Id, storyTellerId);

        if (CurrentStoryTeller.HasSubmittedCardStory())
            throw new StoryAlreadySubmittedException(Id, storyTellerId);

        var card = GetCurrentStoryTellerHand().SubtractCard(cardId);
        SubmittedCards.Add(new SubmittedCard(storyTellerId, card, Id));
        CurrentStoryTeller = CurrentStoryTeller.SubmitStory(story);

        AddDomainEvent(new StoryTellerCardStorySubmitted(Id, storyTellerId, cardId, story));
    }

    public void SubmitGuessingPlayerCard(PlayerId guessingPlayerId, CardId cardId)
    {
        EnsureGameIsInProgress(guessingPlayerId);
        EnsureCurrentGuessingPlayersContains(guessingPlayerId);

        if (!CurrentStoryTeller.HasSubmittedCardStory())
            throw new GuessingPlayerCannotSubmitCardIfStoryTellerHaveNotSubmitStoryException(Id, guessingPlayerId);

        var playerHand = PlayerHands.Single(x => x.PlayerId == guessingPlayerId);

        var card = playerHand.SubtractCard(cardId);
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

            PlayerHands.Add(new PlayerHand(new PlayerHandId(Guid.NewGuid()), player, cardsToDeal, Id));
        }
    }

    public void VoteCard(PlayerId votingPlayerId, CardId submittedCardId)
    {
        EnsureGameIsInProgress(votingPlayerId);

        if (SubmittedCards.Count < PlayerIds.Count)
            throw new CannotVoteIfAnyPlayerIsPendingToSubmitCardException(Id);

        EnsureCurrentGuessingPlayersContains(votingPlayerId);

        var playersWhoAlreadyVoted = SubmittedCards.SelectMany(x => x.Voters).ToHashSet();
        if (playersWhoAlreadyVoted.Contains(votingPlayerId))
            throw new PlayerAlreadyVotedException(Id, votingPlayerId);

        var submittedCard = SubmittedCards.SingleOrDefault(x => x.Card.Id == submittedCardId)
            ?? throw new CardNotFoundInSubmittedCardsException(Id, submittedCardId);

        submittedCard.RecordVote(votingPlayerId);
        AddDomainEvent(new GuessingPlayerVoted(Id, votingPlayerId, submittedCard.Card.Id));
        
        if (HaveAllPlayersVoted())
            CompleteCurrentRound();
    }

    public void LinkToNextGameRoom(GameRoomId nextGameRoomId)
    {
        if (Status != GameStatus.Finished)
            throw new CannotLinkToNextGameRoomIfItsNotFinishedException(Id);
        
        NextGameRoomId = nextGameRoomId;
        
        AddDomainEvent(new NextGameRoomLinked(Id, nextGameRoomId));
    }

    public bool IsLinkedToNextGameRoom() => NextGameRoomId != GameRoomId.Empty;

    private PlayerHand GetCurrentStoryTellerHand() => PlayerHands.Single(x => x.PlayerId == CurrentStoryTeller.PlayerId);

    private void EnsurePlayersContains(PlayerId playerId)
    {
        if (!PlayerIds.Contains(playerId))
            throw new PlayerNotInGameRoomException(Id, playerId);
    }

    private void EnsureGameIsInProgress(PlayerId playerIdExecutingTheAction)
    {
        if (Status != GameStatus.InProgress)
            throw new InvalidOperationForNotInProgressGameException(Id, playerIdExecutingTheAction);
    }
    
    private void EnsureCurrentGuessingPlayersContains(PlayerId playerId)
    {
        if (!GetCurrentGuessingPlayerIds().Contains(playerId))
            throw new PlayerNotFoundInCurrentGuessingPlayersException(playerId);
    }

    private bool HaveAllPlayersVoted() => SubmittedCards.SelectMany(x => x.Voters).Count() == PlayerCount - 1;

    private void CompleteCurrentRound()
    {
        var pointsByPlayer = CalculateScoresOfCurrentRound();
        
        FinishedRounds.Add(new FinishedRound(Id, DateTimeOffset.UtcNow, pointsByPlayer.AsReadOnly(), SubmittedCards.ToArray(), CurrentStoryTeller));

        if (CurrentCardReRolls.Count > 0)
        {
            var unusedReservedCardImageUrls = CurrentCardReRolls.SelectMany(x => x.ReservedCards).Select(x => x.Url).ToArray();
            if (unusedReservedCardImageUrls.Length > 0)
                AddDomainEvent(new ReserveCardsForReRollDiscarded(Id, unusedReservedCardImageUrls));
        }
        
        var gameHasFinished = FinishedRounds.Count == PlayerCount;
        if (gameHasFinished)
        {
            Status = GameStatus.Finished;
            AddDomainEvent(new GameFinished(Id));
        }
        else
        {
            ShiftToNextRound();
        }
    }

    private Dictionary<PlayerId, Points> CalculateScoresOfCurrentRound()
    {
        var pointsByPlayer = PlayerIds.ToDictionary(x => x, _ => Points.Zero);
        var submittedCardsByPlayerId = SubmittedCards.ToDictionary(x => x.PlayerId);

        var amountOfPlayersWhoVotedStoryTellerCard = submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Count;
        if (amountOfPlayersWhoVotedStoryTellerCard > 0 && amountOfPlayersWhoVotedStoryTellerCard < GetCurrentGuessingPlayerIds().Count)
            pointsByPlayer[CurrentStoryTeller.PlayerId] += new Points(3);

        foreach (var guessingPlayerId in GetCurrentGuessingPlayerIds())
        {
            if (submittedCardsByPlayerId[CurrentStoryTeller.PlayerId].Voters.Contains(guessingPlayerId))
                pointsByPlayer[guessingPlayerId] += new Points(3);

            pointsByPlayer[guessingPlayerId] += new Points(submittedCardsByPlayerId[guessingPlayerId].Voters.Count);
        }

        AddDomainEvent(new VotingScoresComputed(Id));
        return pointsByPlayer;
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
        CurrentCardReRolls.Clear();
        
        AddDomainEvents(new NewRoundStarted(Id));
    }

    public void ReserveCardsForReRoll(PlayerId playerId, Card[] cards)
    {
        EnsureGameIsInProgress(playerId);
        EnsurePlayersContains(playerId);

        if (CurrentCardReRolls.Any(x => x.PlayerId == playerId))
            throw new OnlyOneReRollAvailablePerRoundException(Id, playerId);
        
        CurrentCardReRolls.Add(new CardReRoll(new(Guid.NewGuid()), playerId, cards));

        AddDomainEvent(new CardReRollReserved(Id, playerId));
    }

    public void SelectCardToReRoll(PlayerId reRollingPlayerId, CardId cardToReRollId, CardId newCardId)
    {
        EnsureGameIsInProgress(reRollingPlayerId);
        EnsurePlayersContains(reRollingPlayerId);
        
        var cardReRoll = CurrentCardReRolls.FirstOrDefault(x => x.PlayerId == reRollingPlayerId)
            ?? throw new CardReRollNotReservedException(Id, reRollingPlayerId);

        if (cardReRoll.IsCompleted)
            throw new ReRollAlreadyCompletedThisRoundException(Id, reRollingPlayerId);
        
        var newCard = cardReRoll.ReservedCards.FirstOrDefault(x => x.Id == newCardId) 
            ?? throw new NewCardNotFoundInReservedCardsToReRollException(Id, reRollingPlayerId, newCardId);
        
        var reRollingPlayerHand = PlayerHands.First(x => x.PlayerId == reRollingPlayerId);
        reRollingPlayerHand.RemoveCard(cardToReRollId);
        reRollingPlayerHand.AddCard(newCard);
        
        var discardedCards = cardReRoll.ReservedCards.Where(x => x.Id != newCard.Id).ToArray();
        AddDomainEvent(new ReserveCardsForReRollDiscarded(Id, discardedCards.Select(x => x.Url).ToArray()));
        
        cardReRoll.Complete();
        AddDomainEvent(new CardReRollCompleted(Id, reRollingPlayerId, cardToReRollId, newCardId));
    }

    public void MarkAsPublic(PlayerId playerId)
    {
        EnsurePlayersContains(playerId);

        if (Status != GameStatus.NotStarted)
            throw new InvalidOperationForStartedGameException(Id, playerId);

        if (CreatedBy != playerId)
            throw new GamePermissionDeniedException(Id, playerId);

        if (IsPublic)
            return;

        IsPublic = true;
        AddDomainEvent(new GameRoomMarkedAsPublic(Id));
    }

    public void MarkAsPrivate(PlayerId playerId)
    {
        EnsurePlayersContains(playerId);

        if (Status != GameStatus.NotStarted)
            throw new InvalidOperationForStartedGameException(Id, playerId);

        if (CreatedBy != playerId)
            throw new GamePermissionDeniedException(Id, playerId);

        if (!IsPublic)
            return;

        IsPublic = false;
        AddDomainEvent(new GameRoomMarkedAsPrivate(Id));
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

    public long GetLongHashCode()
    {
        var guid = Value;
        Span<byte> destination = stackalloc byte[16];
        MemoryMarshal.Write(destination, in guid);
        ref var firstPartOfGuidAsLong = ref MemoryMarshal.AsRef<long>(destination);
        
        return firstPartOfGuidAsLong ^ Unsafe.Add(ref firstPartOfGuidAsLong, 1);
    }
}

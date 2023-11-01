using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Tests.Common;

public sealed class GameRoomBuilder
{
    private GameRoomId _id = new(Guid.NewGuid());
    private PlayerId _creatorId = new(Guid.NewGuid().ToString());
    private PlayerId[] _playersThatJoined = Array.Empty<PlayerId>();
    private DateTimeOffset _createdAt = new(2023, 1, 1, 2, 3, 4, TimeSpan.Zero);
    private CardBuilder[] _cards = Array.Empty<CardBuilder>();
    private bool _isStarted = false;
    private string _storyTellerCardStory = string.Empty;
    private IEnumerable<PlayerId> _guessingPlayersThatSubmittedCard = Enumerable.Empty<PlayerId>();
    private List<(PlayerId VotingPlayerId, PlayerId VotedCardSubmitter)> _votes = new();
    private bool _withNoCardsLeftInDeck = false;

    public GameRoom Build()
    {
        var gameRoom = new GameRoom(_id, _creatorId, _createdAt);

        foreach (var player in _playersThatJoined)
            gameRoom.Join(player);

        if (_cards.Any()) 
            gameRoom.AssignDeck(_cards.Select(x => x.Build()).ToArray());
        
        if (_isStarted)
        {
            gameRoom.Start(_creatorId);
        }

        if (!string.IsNullOrEmpty(_storyTellerCardStory))
        {
            var storyTellerId = gameRoom.CurrentStoryTeller.PlayerId;
            var card = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First();

            gameRoom.SubmitStory(storyTellerId, card.Id, _storyTellerCardStory);
        }

        foreach (var guessingPlayerId in _guessingPlayersThatSubmittedCard)
        {
            var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
            gameRoom.SubmitGuessingPlayerCard(guessingPlayerId, card.Id);
        }

        if (_withNoCardsLeftInDeck)
            gameRoom.Deck.Clear();

        foreach (var vote in _votes)
        {
            var cardId = gameRoom.SubmittedCards.Single(x => x.PlayerId == vote.VotedCardSubmitter).Card.Id;
            gameRoom.VoteCard(vote.VotingPlayerId, cardId);
        }

        gameRoom.ClearDomainEvents();
        return gameRoom;
    }

    public static GameRoomBuilder CreateStarted(GameRoomId gameRoomId, PlayerId creator, PlayerId[] playersThatJoined)
    {
        var gameRoomBuilder = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(playersThatJoined)
            .WithAssignedDeck(Enumerable.Range(0, new TotalNumberOfCardsRequired(playersThatJoined.Length + 1).RequiredCardCount)
                .Select(_ => new CardBuilder())
                .ToArray())
            .Started();
        return gameRoomBuilder;
    }

    public static GameRoomBuilder CreateFinished(GameRoomId gameRoomId, PlayerId creator, PlayerId[] playersThatJoined)
    {
        var gameRoomBuilder = CreateStarted(gameRoomId, creator, playersThatJoined)
            .WithoutCardsLeftInDeck()
            .WithStoryTellerStory("Any story")
            .WithGuessingPlayerThatSubmittedCard(playersThatJoined[0], playersThatJoined[1])
            .WithVote(playersThatJoined[0], creator)
            .WithVote(playersThatJoined[1], creator);
        return gameRoomBuilder;

    }

    public GameRoomBuilder WithId(GameRoomId id)
    {
        _id = id;
        return this;
    }

    public GameRoomBuilder WithCreator(PlayerId creatorId)
    {
        _creatorId = creatorId;
        return this;
    }

    public GameRoomBuilder WithPlayersThatJoined(params PlayerId[] playersThatJoined)
    {
        ArgumentNullException.ThrowIfNull(playersThatJoined);

        _playersThatJoined = playersThatJoined;
        return this;
    }

    public GameRoomBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public GameRoomBuilder WithAssignedDeck(params CardBuilder[] cards)
    {
        _cards = cards;
        return this;
    }

    public GameRoomBuilder WithCardsInDeck(int count)
    {
        WithAssignedDeck(Enumerable.Range(0, count).Select(_ => new CardBuilder()).ToArray());
        return this;
    }
    
    public GameRoomBuilder WithValidDeckAssigned()
    {
        WithAssignedDeck(Enumerable.Range(0, new TotalNumberOfCardsRequired(_playersThatJoined.Length + 1).RequiredCardCount)
            .Select(_ => new CardBuilder())
            .ToArray());
        return this;
    }
    
    public GameRoomBuilder Started()
    {
        _isStarted = true;
        return this;
    }

    public GameRoomBuilder WithStoryTellerStory(string story)
    {
        _storyTellerCardStory = story;
        return this;
    }

    public GameRoomBuilder WithGuessingPlayerThatSubmittedCard(params PlayerId[] playerIds)
    {
        _guessingPlayersThatSubmittedCard = playerIds;
        return this;
    }

    public GameRoomBuilder WithVote(PlayerId votingPlayer, PlayerId cardSubmittedBy)
    {
        _votes.Add((votingPlayer, cardSubmittedBy));
        return this;
    }

    public GameRoomBuilder WithoutCardsLeftInDeck()
    {
        _withNoCardsLeftInDeck = true;
        return this;
    }
}

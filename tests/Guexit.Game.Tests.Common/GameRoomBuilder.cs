using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Tests.Common;

public sealed class GameRoomBuilder
{
    private GameRoomId _id = new(Guid.NewGuid());
    private PlayerId _creatorId = new(Guid.NewGuid().ToString());
    private PlayerId[] _playersThatJoined = Array.Empty<PlayerId>();
    private DateTimeOffset _createdAt = new(2023, 1, 1, 2, 3, 4, TimeSpan.Zero);
    private RequiredMinPlayers _minRequiredPlayers = RequiredMinPlayers.Default;
    private CardBuilder[] _cards = Array.Empty<CardBuilder>();
    private bool _isStarted = false;
    private string _storyTellerCardStory = string.Empty;
    private IEnumerable<PlayerId> _guessingPlayersThatSubmittedCard = Enumerable.Empty<PlayerId>();
    private (PlayerId VotingPlayerId, CardId VotedCardId)[] _votes = Array.Empty<(PlayerId VotingPlayerId, CardId VotedCardId)>();

    public GameRoom Build()
    {
        var gameRoom = new GameRoom(_id, _creatorId, _createdAt);

        gameRoom.DefineMinRequiredPlayers(_minRequiredPlayers.Count);

        foreach (var player in _playersThatJoined)
            gameRoom.Join(player);

        if (_isStarted) 
            gameRoom.Start();
        
        if (_cards.Any()) 
            gameRoom.AssignDeck(_cards.Select(x => x.Build()).ToArray());

        if (!string.IsNullOrEmpty(_storyTellerCardStory))
        {
            var storyTellerId = gameRoom.CurrentStoryTeller.PlayerId;
            var card = gameRoom.PlayerHands.Single(x => x.PlayerId == storyTellerId).Cards.First();

            gameRoom.SubmitStoryTellerCardStory(storyTellerId, card.Id, _storyTellerCardStory);
        }

        foreach (var guessingPlayerId in _guessingPlayersThatSubmittedCard)
        {
            var card = gameRoom.PlayerHands.Single(x => x.PlayerId == guessingPlayerId).Cards.First();
            gameRoom.SubmitGuessingPlayerCard(guessingPlayerId, card.Id);
        }

        gameRoom.ClearDomainEvents();
        return gameRoom;
    }

    public static GameRoomBuilder CreateStarted(GameRoomId gameRoomId, PlayerId creator, PlayerId[] playersThatJoined)
    {
        var allPlayers = playersThatJoined.Concat(new[] { creator });
        if (!RequiredMinPlayers.Default.AreSatisfiedBy(allPlayers.Count()))
            throw new InvalidOperationException(
                $"Cannot build a game room with {allPlayers.Count()} players. Minimum are {RequiredMinPlayers.Default.Count}");

        var gameRoomBuilder = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(playersThatJoined)
            .WithDeck(Enumerable.Range(0, allPlayers.Count() * GameRoom.TotalCardsPerPlayer).Select(_ => new CardBuilder()).ToArray())
            .Started();
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
        _playersThatJoined = playersThatJoined;
        return this;
    }

    public GameRoomBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public GameRoomBuilder WithMinRequiredPlayers(int count)
    {
        _minRequiredPlayers = new RequiredMinPlayers(count);
        return this;
    }

    public GameRoomBuilder WithDeck(params CardBuilder[] cards)
    {
        _cards = cards;
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
}

﻿using System.Linq;
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

    public GameRoom Build()
    {
        var gameRoom = new GameRoom(_id, _creatorId, _createdAt);

        foreach (var player in _playersThatJoined)
            gameRoom.Join(player);

        gameRoom.DefineMinRequiredPlayers(_minRequiredPlayers.Count);

        if (_isStarted)
            gameRoom.Start();

        if (_cards.Any())
            gameRoom.AssignDeck(_cards.Select(x => x.Build()));
        
        gameRoom.ClearDomainEvents();
        return gameRoom;
    }

    public static GameRoomBuilder CreateStarted(GameRoomId gameRoomId, PlayerId creator, PlayerId[] playersThatJoined)
    {
        var allPlayers = playersThatJoined.Concat(new[] { creator }).ToArray();
        if (!RequiredMinPlayers.Default.AreSatisfiedBy(allPlayers.Length))
        {
            throw new InvalidOperationException($"Cannot build a game room with {allPlayers.Length} players. Minimum are {RequiredMinPlayers.Default.Count}");
        }

        var gameRoomBuilder = new GameRoomBuilder()
            .WithId(gameRoomId)
            .WithCreator(creator)
            .WithPlayersThatJoined(playersThatJoined)
            .WithDeck(Enumerable.Range(0, allPlayers.Length * GameRoom.TotalCardsPerPlayer).Select(_ => new CardBuilder()).ToArray())
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
}

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

    public GameRoom Build()
    {
        var gameRoom = new GameRoom(_id, _creatorId, _createdAt);

        foreach (var player in _playersThatJoined)
        {
            gameRoom.Join(player);
        }

        gameRoom.DefineMinRequiredPlayers(_minRequiredPlayers.Count);
        gameRoom.ClearDomainEvents();
        return gameRoom;
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
}

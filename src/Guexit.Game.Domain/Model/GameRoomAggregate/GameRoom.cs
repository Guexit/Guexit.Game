using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.PlayerAggregate;

namespace Guexit.Game.Domain.Model.GameRoomAggregate;

public sealed class GameRoom : AggregateRoot<GameRoomId>
{
    public ICollection<PlayerId> PlayerIds { get; private set; } = new List<PlayerId>();
    public DateTimeOffset CreatedAt { get; private set; }
    public RequiredMinPlayers RequiredMinPlayers { get; private set; } = RequiredMinPlayers.Default;
    public GameStatus Status { get; private set; } = GameStatus.NotStarted;
    
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
}

using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Model.ImageAggregate;

public sealed class Image : AggregateRoot<ImageId>
{
    public GameRoomId GameRoomId { get; private set; } = GameRoomId.Empty;
    public Uri Url { get; private set; } = default!;
    public int LogicalShard { get; private set; } 
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsAssignedToGameRoom => GameRoomId is not null;

    private Image() { /* Entity Framework required parameterless ctor*/ }

    public Image(ImageId id, Uri url, int logicalShard, DateTimeOffset createdAt)
    {
        Id = id;
        Url = url;
        LogicalShard = logicalShard;
        CreatedAt = createdAt;
    }

    public void AssignToGame(GameRoomId gameRoomId)
    {
        if (GameRoomId != GameRoomId.Empty)
        {
            throw new Exception();
        }

        GameRoomId = gameRoomId;
    }
}

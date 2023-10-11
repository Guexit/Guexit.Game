using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Model.ImageAggregate;

public sealed class Image : AggregateRoot<ImageId>
{
    public GameRoomId GameRoomId { get; private set; } = GameRoomId.Empty;
    public Uri Url { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsAssignedToGameRoom => GameRoomId is not null;

    private Image() { /* Entity Framework required parameterless ctor*/ }

    public Image(ImageId id, Uri url, DateTimeOffset createdAt)
    {
        Id = id;
        Url = url;
        CreatedAt = createdAt;
    }

    public void AssignTo(GameRoomId gameRoomId)
    {
        if (GameRoomId != GameRoomId.Empty)
        {
            throw new Exception();
        }

        GameRoomId = gameRoomId;
    }
}

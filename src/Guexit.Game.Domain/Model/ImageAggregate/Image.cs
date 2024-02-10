using Guexit.Game.Domain.Exceptions;
using Guexit.Game.Domain.Model.GameRoomAggregate;

namespace Guexit.Game.Domain.Model.ImageAggregate;

public sealed class Image : AggregateRoot<ImageId>
{
    public GameRoomId GameRoomId { get; private set; } = GameRoomId.Empty;
    public Uri Url { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public ISet<Tag> Tags { get; private init; } = new HashSet<Tag>();
    
    public bool IsAssignedToAGameRoom => GameRoomId != GameRoomId.Empty;

    private Image() { /* Entity Framework required parameterless ctor */ }

    public Image(ImageId id, Uri url, IEnumerable<Tag> tags, DateTimeOffset createdAt)
    {
        Id = id;
        Url = url;
        CreatedAt = createdAt;
        Tags = new HashSet<Tag>(tags);
    }

    public void AssignTo(GameRoomId gameRoomId)
    {
        if (GameRoomId != GameRoomId.Empty)
            throw new ImageAlreadyAssignedToGameRoomException(Id);

        GameRoomId = gameRoomId;
    }
}

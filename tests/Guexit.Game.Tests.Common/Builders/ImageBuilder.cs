using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Tests.Common.Builders;

public sealed class ImageBuilder
{
    private ImageId _id = new(Guid.NewGuid());
    private Uri _url = new("https://example.com/image.png");
    private Tag[] _tags = [];
    private DateTimeOffset _createdAt = new(2023, 1, 1, 2, 3, 4, TimeSpan.Zero);
    private GameRoomId _gameRoomId = GameRoomId.Empty;

    public static ImageBuilder CreateValid()
    {
        var imageId = new ImageId(Guid.NewGuid());
        return new ImageBuilder()
            .WithId(imageId)
            .WithTags(new Tag[] { new("model:turbo_v1"), new("style:comic") })
            .WithUrl(new Uri($"https://guexit.io/images/{imageId.Value}"));
    }

    public Image Build()
    {
        var image = new Image(_id, _url, _tags, _createdAt);

        if (_gameRoomId != GameRoomId.Empty)
            image.AssignTo(_gameRoomId);
        
        return image;
    }

    public ImageBuilder WithId(Guid id) => WithId(new ImageId(id));
    public ImageBuilder WithId(ImageId id)
    {
        _id = id;
        return this;
    }

    public ImageBuilder WithUrl(Uri url)
    {
        _url = url;
        return this;
    }

    public ImageBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ImageBuilder WithGameRoomId(GameRoomId gameRoomId)
    {
        _gameRoomId = gameRoomId;
        return this;
    }

    public ImageBuilder WithTags(IEnumerable<string> tags) => WithTags(tags.Select(x => new Tag(x)).ToArray());
    public ImageBuilder WithTags(Tag[] tags)
    {
        _tags = tags;
        return this;
    }
}
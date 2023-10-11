using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Tests.Common;

public sealed class ImageBuilder
{
    private ImageId _id = new(Guid.NewGuid());
    private Uri _url = new("https://example.com/image.png");
    private DateTimeOffset _createdAt = new(2023, 1, 1, 2, 3, 4, TimeSpan.Zero);

    public static ImageBuilder CreateValid()
    {
        var imageId = new ImageId(Guid.NewGuid());
        return new ImageBuilder()
            .WithId(imageId)
            .WithUrl(new Uri($"https://guexit.io/images/{imageId.Value}"));
    }
    
    public Image Build()
    {
        var image = new Image(_id, _url, _createdAt);

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
}
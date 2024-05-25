using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.UnitTests.Repositories;

public sealed class FakeInMemoryImageRepository : IImageRepository
{
    private readonly Dictionary<ImageId, Image> _images = new();

    public ValueTask Add(Image gameRoom, CancellationToken ct = default)
    {
        _images.Add(gameRoom.Id, gameRoom);
        return ValueTask.CompletedTask;
    }

    public ValueTask AddRange(IEnumerable<Image> images, CancellationToken _ = default)
    {
        foreach (var image in images)
        {
            _images.Add(image.Id, image);
        }

        return ValueTask.CompletedTask;
    }

    public Task<int> CountAvailableImages(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_images.Values.Count(x => x.GameRoomId == GameRoomId.Empty));
    }

    public Task<Image[]> GetAvailableImages(int limit, CancellationToken ct = default)
    {
        return Task.FromResult(_images.Values.Take(limit)
            .Where(x => x.GameRoomId == GameRoomId.Empty).ToArray());
    }

    public Task<Image[]> GetBy(IEnumerable<Uri> imageUrls, CancellationToken ct)
    {
        var imageUrlsHashSet = new HashSet<Uri>(imageUrls);
        return Task.FromResult(_images.Values.Where(x => imageUrlsHashSet.Contains(x.Url)).ToArray());
    }

    public Task<Image?> GetBy(ImageId imageId, CancellationToken ct = default)
    {
        return Task.FromResult(_images.GetValueOrDefault(imageId));
    }
}


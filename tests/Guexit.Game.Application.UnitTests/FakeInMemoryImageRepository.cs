using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class FakeInMemoryImageRepository : IImageRepository
{
    private readonly Dictionary<ImageId, Image> _imagesById = new();

    public ValueTask Add(Image gameRoom, CancellationToken ct = default)
    {
        _imagesById.Add(gameRoom.Id, gameRoom);
        return ValueTask.CompletedTask;
    }

    public ValueTask AddRange(IEnumerable<Image> images, CancellationToken cancellationToken = default)
    {
        foreach (var image in images)
        {
            _imagesById.Add(image.Id, image);
        }

        return ValueTask.CompletedTask;
    }

    public Task<int> CountAvailableImages(int logicalShard, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_imagesById.Values.Where(x => x.LogicalShard == logicalShard).Count());
    }

    public Task<Image[]> GetAvailableImages(int take, int logicalShard, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_imagesById.Values.Take(take)
            .Where(x => x.GameRoomId == GameRoomId.Empty && x.LogicalShard == logicalShard).ToArray());
    }

    public Task<Image?> GetBy(ImageId imageId, CancellationToken ct = default)
    {
        return Task.FromResult(_imagesById.GetValueOrDefault(imageId));
    }
}


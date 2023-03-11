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

    public Task<Image?> GetBy(ImageId imageId, CancellationToken ct = default)
    {
        return Task.FromResult(_imagesById.GetValueOrDefault(imageId));
    }
}


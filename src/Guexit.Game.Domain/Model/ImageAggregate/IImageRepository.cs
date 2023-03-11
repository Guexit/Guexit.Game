namespace Guexit.Game.Domain.Model.ImageAggregate;

public interface IImageRepository
{
    ValueTask Add(Image image, CancellationToken cancellationToken = default);
    Task<Image?> GetBy(ImageId imageId, CancellationToken cancellationToken = default);
}

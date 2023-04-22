﻿namespace Guexit.Game.Domain.Model.ImageAggregate;

public interface IImageRepository
{
    ValueTask Add(Image image, CancellationToken cancellationToken = default);
    ValueTask AddRange(IEnumerable<Image> images, CancellationToken cancellationToken = default);
    Task<Image?> GetBy(ImageId imageId, CancellationToken cancellationToken = default);
    Task<int> CountAvailableImages(CancellationToken cancellationToken = default);
    Task<Image[]> GetAvailableImages(int amount, CancellationToken cancellationToken = default);
}   

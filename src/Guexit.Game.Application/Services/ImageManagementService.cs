﻿using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.Services;

public interface IImageManagementService
{
    Task AddImage(Guid imageId, Uri url, string[] tags, CancellationToken cancellationToken = default);
}

public sealed class ImageManagementService : IImageManagementService
{
    private readonly IImageRepository _imageRepository;
    private readonly ISystemClock _clock;

    public ImageManagementService(IImageRepository imageRepository, ISystemClock clock)
    {
        _imageRepository = imageRepository;
        _clock = clock;
    }

    public async Task AddImage(Guid id, Uri url, string[] tags, CancellationToken cancellationToken = default)
    {
        var image = new Image(new ImageId(id), url, tags.Select(x => new Tag(x)).ToArray(), _clock.UtcNow);
        
        await _imageRepository.Add(image, cancellationToken);
    }
}
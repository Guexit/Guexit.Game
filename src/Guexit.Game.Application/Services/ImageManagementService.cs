using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.Services;

public interface IImageManagementService
{
    Task AddImage(Guid imageId, Uri url, CancellationToken cancellationToken = default);
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

    public async Task AddImage(Guid id, Uri url, CancellationToken cancellationToken = default)
    {
        var imageId = new ImageId(id);
        if (await _imageRepository.GetBy(imageId, cancellationToken) is not null) 
            return;

        var image = new Image(imageId, url,  _clock.UtcNow);
        await _imageRepository.Add(image, cancellationToken);
    }
}
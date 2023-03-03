using Guexit.Game.Application.CardAssigment;
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
    private readonly ILogicalShardProvider _logicalShardProvider;

    public ImageManagementService(IImageRepository imageRepository, ISystemClock systemClock, ILogicalShardProvider logicalShardProvider)
    {
        _imageRepository = imageRepository;
        _clock = systemClock;
        _logicalShardProvider = logicalShardProvider;
    }

    public async Task AddImage(Guid id, Uri url, CancellationToken cancellationToken = default)
    {
        var imageId = new ImageId(id);

        var existingImage = await _imageRepository.GetBy(imageId);
        if (existingImage is not null) 
            return;

        var image = new Image(imageId, url, _logicalShardProvider.GetLogicalShard(), _clock.UtcNow);
        await _imageRepository.Add(image, cancellationToken);
    }
}
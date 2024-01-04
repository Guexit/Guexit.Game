using Guexit.Game.Application.Services;
using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenAddingImage
{
    private static readonly DateTimeOffset UtcNow = new(2023, 3, 3, 7, 9, 0, TimeSpan.Zero);

    private readonly ISystemClock _systemClock = Substitute.For<ISystemClock>();
    private readonly IImageRepository _imageRepository = new FakeInMemoryImageRepository();
    private readonly IImageManagementService _imageManagementService;
    
    public WhenAddingImage()
    {
        _systemClock.UtcNow.Returns(UtcNow);
        _imageManagementService = new ImageManagementService(_imageRepository, _systemClock);
    }

    [Fact]
    public async Task ImageIsAdded()
    {
        var imageId = new ImageId(Guid.NewGuid());
        var url = new Uri("https://guexit.ai/images/imagename");

        await _imageManagementService.AddImage(imageId.Value, url);

        var image = await _imageRepository.GetBy(imageId);
        image.Should().NotBeNull();
        image!.Id.Should().Be(imageId);
        image.Url.Should().Be(url);
        image.CreatedAt.Should().Be(UtcNow);
    }
}

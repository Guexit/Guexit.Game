using Guexit.Game.Application.DomainEventHandlers;
using Guexit.Game.Application.UnitTests.Repositories;
using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.ImageAggregate;
using Guexit.Game.Tests.Common.Builders;

namespace Guexit.Game.Application.UnitTests;

public sealed class WhenHandlingReserveCardsForReRollDiscarded
{
    private static readonly GameRoomId AnyGameRoomId = new(Guid.NewGuid());
    
    private readonly IImageRepository _imageRepository;
    private readonly ReturnUnusedImagesToAvailablePoolOnReserveCardsForReRollDiscarded _eventHandler;

    public WhenHandlingReserveCardsForReRollDiscarded()
    {
        _imageRepository = new FakeInMemoryImageRepository();
        _eventHandler = new ReturnUnusedImagesToAvailablePoolOnReserveCardsForReRollDiscarded(_imageRepository);
    }

    [Fact]
    public async Task UnusedImagesAreReturnedToTheAvailableImagePool()
    {
        var image1 = new ImageBuilder()
            .WithGameRoomId(AnyGameRoomId)
            .WithUrl(new("https://guexit.com/unused-card-reserved-imade-for-re-roll-1"))
            .Build();
        var image2 = new ImageBuilder()
            .WithGameRoomId(AnyGameRoomId)
            .WithUrl(new("https://guexit.com/unused-card-reserved-imade-for-re-roll-2"))
            .Build();
        await _imageRepository.AddRange([image1, image2]);
        var unusedCardImagesUrls = new[] { image1.Url, image2.Url };
        var @event = new ReserveCardsForReRollDiscarded(AnyGameRoomId, unusedCardImagesUrls);

        await _eventHandler.Handle(@event);

        image1.GameRoomId.Should().Be(GameRoomId.Empty);
        image1.IsAssignedToAGameRoom.Should().BeFalse();
        
        image2.GameRoomId.Should().Be(GameRoomId.Empty);
        image2.IsAssignedToAGameRoom.Should().BeFalse();
    }
}
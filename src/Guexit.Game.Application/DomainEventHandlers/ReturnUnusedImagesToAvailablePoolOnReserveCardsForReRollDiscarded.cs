using Guexit.Game.Domain;
using Guexit.Game.Domain.Model.GameRoomAggregate.Events;
using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.DomainEventHandlers;

public sealed class ReturnUnusedImagesToAvailablePoolOnReserveCardsForReRollDiscarded 
    : IDomainEventHandler<ReserveCardsForReRollDiscarded>
{
    private readonly IImageRepository _imageRepository;

    public ReturnUnusedImagesToAvailablePoolOnReserveCardsForReRollDiscarded(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }
    
    public async ValueTask Handle(ReserveCardsForReRollDiscarded @event, CancellationToken ct = default)
    {
        var images = await _imageRepository.GetBy(@event.UnusedCardImageUrls, ct);

        foreach (var image in images) 
            image.UnassignFromGameRoom();
    }
}
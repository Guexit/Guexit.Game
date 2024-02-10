using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Domain.Exceptions;

public sealed class ImageAlreadyAssignedToGameRoomException : DomainException
{
    public override string Title => "Image is already assigned to a game room";

    public ImageAlreadyAssignedToGameRoomException(ImageId imageId) 
        : base($"Image with id {imageId.Value} is already assigned to a game room. Images cannot be reused across multiple game rooms")
    {
    }
}
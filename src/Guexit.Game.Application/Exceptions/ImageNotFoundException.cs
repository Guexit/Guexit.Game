using Guexit.Game.Domain.Model.ImageAggregate;

namespace Guexit.Game.Application.Exceptions;

public sealed class ImageNotFoundException : AggregateNotFoundException
{
    public ImageNotFoundException(ImageId imageId) 
        : base($"Image with id {imageId.Value} not found.")
    {
    }
}

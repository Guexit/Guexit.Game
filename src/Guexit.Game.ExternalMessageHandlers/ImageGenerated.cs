using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.ExternalMessageHandlers;

// Important: DO NOT move to a different namespace or change name of the class, it's used my masstransit to match registered urn type name
public sealed record ImageGenerated(string Url);

public sealed class ImageGeneratedHandler : ExternalMessageHandler<ImageGenerated>
{
    private readonly IImageManagementService _imageManagementService;
    private readonly IGuidProvider _guidProvider;

    public ImageGeneratedHandler(
        IImageManagementService imageManagementService, 
        IGuidProvider guidProvider,
        IUnitOfWork unitOfWork, 
        ILogger<ImageGeneratedHandler> logger) : base(unitOfWork, logger)
    {
        _imageManagementService = imageManagementService;
        _guidProvider = guidProvider;
    }

    protected override async Task Process(ImageGenerated message, CancellationToken cancellationToken)
    {
        await _imageManagementService.AddImage(_guidProvider.NewGuid(), new Uri(message.Url));
    }
}
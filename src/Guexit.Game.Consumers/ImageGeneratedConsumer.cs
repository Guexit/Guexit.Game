using Guexit.Game.Application;
using Guexit.Game.Application.Services;
using Guexit.Game.Domain;
using Guexit.Game.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Guexit.Game.Consumers;

public sealed class ImageGeneratedConsumer : MessageConsumer<ImageGenerated>
{
    private readonly IImageManagementService _imageManagementService;
    private readonly IGuidProvider _guidProvider;

    public ImageGeneratedConsumer(
        IImageManagementService imageManagementService, 
        IGuidProvider guidProvider,
        IUnitOfWork unitOfWork, 
        ILogger<ImageGeneratedConsumer> logger) : base(unitOfWork, logger)
    {
        _imageManagementService = imageManagementService;
        _guidProvider = guidProvider;
    }

    protected override async Task Process(ImageGenerated imageGenerated, CancellationToken cancellationToken)
    {
        await _imageManagementService.AddImage(_guidProvider.NewGuid(), new Uri(imageGenerated.Url), 
            imageGenerated.Tags, cancellationToken);
    }
}

public class ImageGeneratedConsumerDefinition : ConsumerDefinition<ImageGeneratedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<ImageGeneratedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        
        endpointConfigurator.ClearSerialization();
        endpointConfigurator.UseRawJsonSerializer();
    }
}
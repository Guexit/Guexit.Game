using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Messages;
using Guexit.Game.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationService
{
    private readonly IBus _bus;
    private readonly GameDbContext _dbContext;
    private readonly ILogger<ImageGenerationService> _logger;
    private readonly IOptions<ImageGenerationBackgroundServiceOptions> _options;

    public ImageGenerationService(IBus bus, GameDbContext dbContext, ILogger<ImageGenerationService> logger, IOptions<ImageGenerationBackgroundServiceOptions> options) 
        => (_bus, _dbContext, _logger, _options) = (bus, dbContext, logger, options);

    public async Task GenerateImages(CancellationToken cancellationToken = default)
    {
        var availableImagesCount = await _dbContext.Images.CountAsync(x => x.GameRoomId == GameRoomId.Empty, cancellationToken);

        _logger.LogInformation("Actual available images count {availableImagesCount}. Target pool size {targetPoolSize}",
            availableImagesCount, _options.Value.TargetAvailableImagePoolSize);

        if (availableImagesCount >= _options.Value.TargetAvailableImagePoolSize)
            return;

        _logger.LogInformation("Sending {commandName}...", nameof(GenerateImagesCommand));
        await _bus.Send(new GenerateImagesCommandWithStyles(), cancellationToken);
    }
}

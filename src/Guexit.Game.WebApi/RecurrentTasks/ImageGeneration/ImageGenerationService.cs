using Guexit.Game.Domain.Model.GameRoomAggregate;
using Guexit.Game.Messages;
using Guexit.Game.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Guexit.Game.WebApi.RecurrentTasks.ImageGeneration;

public sealed class ImageGenerationService
{
    private static readonly (string PositivePrompt, string NegativePrompt)[] Prompts = new[]
    {
        ("A lakeside scene with two silhouettes sharing a secret as the sun sets, casting vibrant colors across the sky.",
        "A bland, uninteresting landscape with dull colors."),
        ("A curious octopus exploring a treasure chest amidst a lively underwater coral reef, teeming with colorful marine life.",
        "A barren underwater environment with minimal life."),
        ("An adventurer standing atop a majestic mountain peak, gazing upon a hidden valley bathed in the warm glow of sunrise.",
        "A flat, featureless landscape with no mountains."),
        ("A cityscape where a mysterious figure in a trench coat dashes through rain-soaked streets, neon signs reflecting in puddles.",
        "A deserted city with no activity or life."),
        ("A Japanese Zen garden where an elderly monk tenderly sweeps the ground beneath blooming cherry blossoms, contemplating the passage of time.",
        "A messy, disorganized garden with no sense of tranquility."),
        ("A tropical rainforest where a vibrant and lively parade of animals and insects celebrate the harmony of nature.",
        "A barren, lifeless forest with no greenery."),
        ("A futuristic cityscape where a group of diverse individuals come together to marvel at the unveiling of a groundbreaking invention.",
        "A dated, rundown city with old architecture."),
        ("An astronaut floating through a vibrant cosmic landscape, reaching out to touch a celestial entity that seems to be alive and aware.",
        "An empty, uninteresting view of space with no celestial bodies."),
        ("A European village where the annual festival brings together rival families, setting aside their differences to celebrate love and unity.",
        "A bland, monotonous village with no charm or character."),
        ("A traveler lost in a snowy landscape, captivated by the dazzling aurora borealis display, as a mysterious figure appears on the horizon.",
        "A dark, featureless night sky with no aurora.")
    };

    private readonly IBus _bus;
    private readonly GameDbContext _dbContext;
    private readonly ILogger<ImageGenerationService> _logger;
    private readonly IOptions<ImageGenerationBackgroundServiceOptions> _options;

    public ImageGenerationService(IBus bus, GameDbContext dbContext, ILogger<ImageGenerationService> logger,
        IOptions<ImageGenerationBackgroundServiceOptions> options)
    {
        (_bus, _dbContext, _logger, _options) = (bus, dbContext, logger, options);
    }

    public async Task GenerateImages(CancellationToken cancellationToken = default)
    {
        var availableImagesCount = await _dbContext.Images.CountAsync(x => x.GameRoomId == GameRoomId.Empty, cancellationToken);

        _logger.LogInformation("Actual available images count {availableImagesCount}. Target pool size {targetPoolSize}",
            availableImagesCount, _options.Value.TargetAvailableImagePoolSize);

        if (availableImagesCount >= _options.Value.TargetAvailableImagePoolSize)
            return;

        _logger.LogInformation("Sending {commandName}...", nameof(GenerateImagesCommand));

        var prompt = Prompts[Random.Shared.Next(Prompts.Length)];
        await _bus.Send(new GenerateImagesCommand
        {
            TextToImage = new TextToImage
            {
                Prompt = new Prompt
                {
                    Positive = prompt.PositivePrompt,
                    Negative = prompt.NegativePrompt
                },
                NumImages = 1
            }
        }, cancellationToken);
    }
}
